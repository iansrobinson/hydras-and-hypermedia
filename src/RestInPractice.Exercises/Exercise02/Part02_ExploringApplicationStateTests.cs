﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise02
{
    [TestFixture]
    public class Part02_ExploringApplicationStateTests
    {
        private static readonly Uri NorthUri = new Uri("/rooms/10", UriKind.Relative);
        private static readonly Uri SouthUri = new Uri("/rooms/11", UriKind.Relative);
        private static readonly Uri EastUri = new Uri("/rooms/12", UriKind.Relative);
        private static readonly Uri WestUri = new Uri("/rooms/13", UriKind.Relative);
        private static readonly Uri BaseUri = new Uri("http://localhost:1234");

        [Test]
        public void ShouldBeNonTerminalState()
        {
            var state = new Exploring(new HttpResponseMessage());
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void ShouldFollowExitToNorthInPreferenceToAllOtherExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .WithSouthLink(SouthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, NorthUri, newResponse));

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ShouldFollowExitToEastIfCannotExitNorth()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithSouthLink(SouthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, EastUri, newResponse));

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ShouldFollowExitToWestIfCannotExitNorthOrEast()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithSouthLink(SouthUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ShouldFollowExitToSouthIfNoOtherExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithSouthLink(SouthUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, SouthUri, newResponse));

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ShouldRememberVisitedExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, NorthUri, new HttpResponseMessage()));

            var state = new Exploring(currentResponse);

            Assert.IsFalse(state.History.Contains(NorthUri));

            var nextState = state.NextState(client);

            Assert.IsTrue(nextState.History.Contains(NorthUri));
        }

        [Test]
        public void ShouldNotChoosePreviouslyChosenExitWhileThereAreOtherExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .WithSouthLink(SouthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var history = new[] {NorthUri, EastUri};

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse, history);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfAllExitsHaveBeenVisitedPreviouslyShouldRetraceStepsSouthInPreferenceToAllOtherExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .WithSouthLink(SouthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var history = new[] {NorthUri, EastUri, WestUri, SouthUri};

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, SouthUri, newResponse));

            var state = new Exploring(currentResponse, history);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfAllExitsHaveBeenVisitedPreviouslyShouldRetraceStepsWestIfCannotExitSouth()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var history = new[] {NorthUri, EastUri, WestUri, SouthUri};

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse, history);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfAllExitsHaveBeenVisitedPreviouslyShouldRetraceStepsEastIfCannotExitSouthOrWest()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .WithEastLink(EastUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var history = new[] {NorthUri, EastUri, WestUri, SouthUri};

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, EastUri, newResponse));

            var state = new Exploring(currentResponse, history);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfAllExitsHaveBeenVisitedPreviouslyShouldRetraceStepsNorthIfNoOtherExits()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithNorthLink(NorthUri)
                .ToString();

            var currentResponse = CreateResponse(entry);
            var newResponse = new HttpResponseMessage();

            var history = new[] {NorthUri, EastUri, WestUri, SouthUri};

            var client = CreateHttpClient(CreateStubEndpoint(BaseUri, NorthUri, newResponse));

            var state = new Exploring(currentResponse, history);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfGoalAchievedShouldTransitionIntoGoalAchievedState()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithTitle("Success")
                .ToString();

            var currentResponse = CreateResponse(entry);

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (GoalAchieved), nextState);
        }

        private static HttpClient CreateHttpClient(StubEndpoint stubEndpoint)
        {
            var client = new HttpClient {Channel = stubEndpoint};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value));
            return client;
        }

        private static StubEndpoint CreateStubEndpoint(Uri baseUri, Uri relativePath, HttpResponseMessage newResponse)
        {
            var requestUri = new Uri(baseUri, relativePath);
            return new StubEndpoint(request => request.RequestUri.Equals(requestUri) ? newResponse : null);
        }

        private static HttpResponseMessage CreateResponse(string entry)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(entry, Encoding.Unicode)};
            currentResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(AtomMediaType.Value);
            return currentResponse;
        }
    }
}