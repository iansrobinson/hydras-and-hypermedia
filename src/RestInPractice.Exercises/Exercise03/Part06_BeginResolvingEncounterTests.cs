using System;
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
    public class Part06_BeginResolvingEncounterTests
    {
        private static readonly Uri Action = new Uri("/encounters/1", UriKind.Relative);
        private static readonly HttpMethod Method = HttpMethod.Post;

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
            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo.WithEndurance(5));
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Exploring), nextState);
        }

        [Test]
        public void IfResponseContainsEncounterFeedWithFormShouldSubmitFormWithCurrentEndurance()
        {
            const int endurance = 5;

            var feed = CreateEncounterFeed();
            var entry = CreateRoundEntry(3);

            var mockEndpoint = new MockEndpoint(CreateResponseWithEntry(entry));
            var client = AtomClient.CreateWithChannel(mockEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo.WithEndurance(endurance));
            initialState.NextState(client);

            var expectedContent = new StringContent("endurance=" + endurance);
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
        public void AfterSubmittingFormShouldReturnNewResolvingEncounterApplicationState()
        {
            var feed = CreateEncounterFeed();
            var entry = CreateRoundEntry(3);

            var stubEndpoint = new StubEndpoint(request => CreateResponseWithEntry(entry));
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo.WithEndurance(5));
            var newState = initialState.NextState(client);

            Assert.IsInstanceOf(typeof (ResolvingEncounter), newState);
        }

        [Test]
        public void NewStateShouldContainRevisedEnduranceAsSpecifiedInResponse()
        {
            const int newEndurance = 3;

            var feed = CreateEncounterFeed();
            var entry = CreateRoundEntry(newEndurance);

            var stubEndpoint = new StubEndpoint(request => CreateResponseWithEntry(entry));
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo.WithEndurance(5));
            var newState = initialState.NextState(client);

            Assert.AreEqual(newEndurance, newState.ApplicationStateInfo.Endurance);
        }
         
        [Test]
        public void IfResponseToSubmittingFormIsNotAtomEntryShouldReturnErrorState()
        {
            var feed = CreateEncounterFeed();
            var secondFeed = CreateEncounterFeed();

            var stubEndpoint = new StubEndpoint(request => CreateResponseWithFeed(secondFeed));
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithFeed(feed), ApplicationStateInfo.WithEndurance(5));
            var newState = initialState.NextState(client);

            Assert.IsInstanceOf(typeof(Error), newState);
        }

        [Test]
        public void ShouldReturnErrorApplicationStateIfCurrentResponseContainsUncategorizedFeed()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo.WithEndurance(5));
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Error), nextState);
        }

        [Test]
        public void ErrorStateForUncategorizedFeedShouldBeInitializedWithCurrentResponse()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateResponseWithFeed(feed);

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo.WithEndurance(5));
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ErrorStateForUncategorizedFeedShouldBeInitializedWithCurrentApplicationStateInfo()
        {
            var applicationStateInfo = ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(new Uri("http://localhost/rooms1")).Build();
            var currentResponse = CreateResponseWithFeed(new FeedBuilder().ToString());

            var initialState = new ResolvingEncounter(currentResponse, applicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(applicationStateInfo, nextState.ApplicationStateInfo);
        }

        [Test]
        public void ShouldReturnErrorApplicationStateIfCurrentResponseContainsNonAtomMediaType()
        {
            var currentResponse = CreateHtmlResponse();

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo.WithEndurance(5));
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Error), nextState);
        }

        [Test]
        public void ErrorStateForNonAtomMediaTypeShouldBeInitializedWithCurrentResponse()
        {
            var currentResponse = CreateHtmlResponse();

            var initialState = new ResolvingEncounter(currentResponse, ApplicationStateInfo.WithEndurance(5));
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ErrorStateForNonAtomMediaTypeShouldBeInitializedWithCurrentApplicationStateInfo()
        {
            var applicationStateInfo = ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(new Uri("http://localhost/rooms1")).Build();

            var currentResponse = CreateHtmlResponse();

            var initialState = new ResolvingEncounter(currentResponse, applicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(applicationStateInfo, nextState.ApplicationStateInfo);
        }

        private static HttpResponseMessage CreateResponseWithFeed(string feed)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(feed)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Feed;
            return currentResponse;
        }

        private static HttpResponseMessage CreateResponseWithEntry(string entry)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(entry)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Entry;
            return currentResponse;
        }

        private static string CreateRoundEntry(int endurance)
        {
            return new EntryBuilder()
                .WithBaseUri(new Uri("http://localhost:8081/"))
                .WithCategory("round")
                .WithForm(new FormWriter(Action, Method, new TextInput("endurance", endurance.ToString())))
                .ToString();
        }

        private static string CreateEncounterFeed()
        {
            return new FeedBuilder()
                .WithBaseUri(new Uri("http://localhost:8081/"))
                .WithCategory("encounter")
                .WithForm(new FormWriter(Action, Method, new TextInput("endurance"))).ToString();
        }

        private static HttpResponseMessage CreateHtmlResponse()
        {
            var currentResponse = new HttpResponseMessage { Content = new StringContent("") };
            currentResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return currentResponse;
        }
    }
}