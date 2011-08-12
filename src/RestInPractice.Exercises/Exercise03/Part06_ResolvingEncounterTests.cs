using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using RestInPractice.Client;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Xhtml;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part06_ResolvingEncounterTests
    {
        private static readonly ApplicationStateInfo ApplicationStateInfo = ApplicationStateInfo.WithEndurance(5).GetBuilder().Build();
        private static readonly Uri SubmissionUri = new Uri("http://localhost:8081/encounters/1");

        [Test]
        public void ShouldBeNonTerminalState()
        {
            var state = new ResolvingEncounter(new HttpResponseMessage(), ApplicationStateInfo.WithEndurance(5));
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void ShouldReturnExploringApplicationStateIfCurrentResponseContainsRoomEntry()
        {
            var feed = new FeedBuilder().WithCategory("room").ToString();
            var initialState = new ResolvingEncounter(CreateCurrentResponseWithFeed(feed), ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Exploring), nextState);
        }

        [Test]
        public void ShouldPostClientEnduranceIfCurrentResponseContainsFeedWithXhtmlForm()
        {
            var expectedMethod = HttpMethod.Post;
            
            var feed = new FeedBuilder().WithCategory("encounter").WithForm(new FormWriter(SubmissionUri, expectedMethod, new TextInput("endurance"))).ToString();
            var entry = new EntryBuilder().WithCategory("round").ToString();
            
            var currentResponse = CreateCurrentResponseWithFeed(feed);
            var nextResponse = CreateCurrentResponseWithEntry(entry);

            var mockEndpoint = new MockEndpoint(nextResponse);

            var client = AtomClient.CreateWithChannel(mockEndpoint);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            initialState.NextState(client);

            Assert.AreEqual(expectedMethod, mockEndpoint.ReceivedRequest.Method);
        }

        [Test]
        public void ShouldReturnErrorApplicationStateIfCurrentResponseContainsUncategorizedFeed()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Error), nextState);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentResponse()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentHistory()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsTrue(nextState.ApplicationStateInfo.History.SequenceEqual(ApplicationStateInfo.History));
        }

        private static HttpResponseMessage CreateCurrentResponseWithFeed(string feed)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(feed)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Feed;
            return currentResponse;
        }

        private static HttpResponseMessage CreateCurrentResponseWithEntry(string entry)
        {
            var currentResponse = new HttpResponseMessage { Content = new StringContent(entry) };
            currentResponse.Content.Headers.ContentType = AtomMediaType.Entry;
            return currentResponse;
        }
    }
}