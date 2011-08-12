using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static readonly Uri Action = new Uri("/encounters/1", UriKind.Relative);
        private static readonly HttpMethod Method = HttpMethod.Post;
        private static readonly TextInput Field = new TextInput("endurance");

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
            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Exploring), nextState);
        }

        [Test]
        public void IfResponseContainsEncounterFeedWithFormShouldSubmitFormWithCurrentEndurance()
        {
            var feed = new FeedBuilder()
                .WithBaseUri(new Uri("http://localhost:8081/"))
                .WithCategory("encounter")
                .WithForm(new FormWriter(Action, Method, Field)).ToString();
            var entry = new EntryBuilder().WithCategory("round").ToString();
            
            var mockEndpoint = new MockEndpoint(CreateResponseWithEntry(entry));
            var client = AtomClient.CreateWithChannel(mockEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo);
            initialState.NextState(client);

            var expectedContent = new StringContent("endurance=5");
            expectedContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var expectedRequest = new HttpRequestMessage
                                      {
                                          Method = HttpMethod.Post, 
                                          RequestUri = new Uri("http://localhost:8081/encounters/1"), 
                                          Content = expectedContent
                                      };

            Assert.IsTrue(HttpRequestComparer.Instance.Equals(expectedRequest, mockEndpoint.ReceivedRequest));
        }

        [Test]
        public void ShouldReturnErrorApplicationStateIfCurrentResponseContainsUncategorizedFeed()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Error), nextState);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentResponse()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentHistory()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsTrue(nextState.ApplicationStateInfo.History.SequenceEqual(ApplicationStateInfo.History));
        }

        private static HttpResponseMessage CreateResponseWithFeed(string feed)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(feed)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Feed;
            return currentResponse;
        }

        private static HttpResponseMessage CreateResponseWithEntry(string entry)
        {
            var currentResponse = new HttpResponseMessage { Content = new StringContent(entry) };
            currentResponse.Content.Headers.ContentType = AtomMediaType.Entry;
            return currentResponse;
        }
    }
}