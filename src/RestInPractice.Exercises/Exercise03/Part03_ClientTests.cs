using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using RestInPractice.Client;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part03_ClientTests
    {
        private static readonly ApplicationStateInfo ApplicationStateInfo = ApplicationStateInfo.WithEndurance(5).GetBuilder().AddToHistory(new Uri("http://localhost/rooms/1")).Build();

        [Test]
        public void ShouldReturnResolvingEncounterApplicationStateIfCurrentResponseContainsEncounterFeed()
        {
            var feed = new FeedBuilder().WithCategory("encounter").ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (ResolvingEncounter), nextState);
        }

        [Test]
        public void ResolvingEncounterStateIsInitializedWithCurrentResponse()
        {
            var feed = new FeedBuilder().WithCategory("encounter").ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ResolvingEncounterStateIsInitializedWithCurrentHistory()
        {
            var feed = new FeedBuilder().WithCategory("encounter").ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsTrue(nextState.ApplicationStateInfo.History.SequenceEqual(ApplicationStateInfo.History));
        }

        [Test]
        public void ShouldReturnErrorApplicationStateIfCurrentResponseContainsUncategorizedFeed()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof (Error), nextState);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentResponse()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }

        [Test]
        public void ErrorStateIsInitializedWithCurrentHistory()
        {
            var feed = new FeedBuilder().ToString();

            var currentResponse = CreateCurrentResponse(feed);

            var initialState = new Exploring(currentResponse, ApplicationStateInfo);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsTrue(nextState.ApplicationStateInfo.History.SequenceEqual(ApplicationStateInfo.History));
        }

        private static HttpResponseMessage CreateCurrentResponse(string feed)
        {
            var currentResponse = new HttpResponseMessage {Content = new StringContent(feed)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.Feed;
            return currentResponse;
        }
    }
}