using System;
using System.Linq;
using System.Net.Http;
using HydrasAndHypermedia.Client;
using HydrasAndHypermedia.Client.ApplicationStates;
using HydrasAndHypermedia.Exercises.Helpers;
using HydrasAndHypermedia.MediaTypes;
using NUnit.Framework;

namespace HydrasAndHypermedia.Exercises.Exercise02
{
    [TestFixture]
    public class Part02_ExploringTests
    {
        private static readonly Uri NorthUri = new Uri("/rooms/10", UriKind.Relative);
        private static readonly Uri SouthUri = new Uri("/rooms/11", UriKind.Relative);
        private static readonly Uri EastUri = new Uri("/rooms/12", UriKind.Relative);
        private static readonly Uri WestUri = new Uri("/rooms/13", UriKind.Relative);
        private static readonly Uri BaseUri = new Uri("http://localhost:1234");

        [Test]
        public void ShouldBeNonTerminalState()
        {
            var state = new Exploring(new HttpResponseMessage(), ApplicationStateInfo.WithEndurance(5));
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, NorthUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, EastUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, SouthUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));
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
            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, NorthUri, new HttpResponseMessage()));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));

            Assert.IsFalse(state.ApplicationStateInfo.History.Contains(NorthUri));

            var nextState = state.NextState(client);

            Assert.IsTrue(nextState.ApplicationStateInfo.History.Contains(NorthUri));
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(history).Build());
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, SouthUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(history).Build());
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, WestUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(history).Build());
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, EastUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(history).Build());
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

            var client = AtomClient.CreateWithChannel(CreateStubEndpoint(BaseUri, NorthUri, newResponse));

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(history).Build());
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }

        [Test]
        public void IfGoalAchievedShouldTransitionIntoGoalAchievedState()
        {
            var entry = new EntryBuilder()
                .WithBaseUri(BaseUri)
                .WithTitle("Exit")
                .ToString();

            var currentResponse = CreateResponse(entry);

            var state = new Exploring(currentResponse, ApplicationStateInfo.WithEndurance(5));
            var nextState = state.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (GoalAchieved), nextState);
        }

        private static StubEndpoint CreateStubEndpoint(Uri baseUri, Uri relativePath, HttpResponseMessage newResponse)
        {
            var requestUri = new Uri(baseUri, relativePath);
            return new StubEndpoint(request => request.RequestUri.Equals(requestUri) ? newResponse : null);
        }

        private static HttpResponseMessage CreateResponse(string entry)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(entry)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Value;
            return currentResponse;
        }
    }
}