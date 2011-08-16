﻿using System;
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
    public class Part07_ResolvingEncounterTests
    {
        private static readonly Uri Action = new Uri("/encounters/1", UriKind.Relative);
        private static readonly HttpMethod Method = HttpMethod.Post;

        [Test]
        public void IfResponseContainsRoundEntryWithPrepopulatedFormShouldSubmitForm()
        {
            const int initialEndurance = 3;
            const int newEndurance = 1;

            var initialEntry = CreateRoundEntry(initialEndurance);
            var newEntry = CreateRoundEntry(newEndurance);

            var mockEndpoint = new MockEndpoint(CreateResponseWithEntry(newEntry));
            var client = AtomClient.CreateWithChannel(mockEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(initialEntry), ApplicationStateInfo.WithEndurance(initialEndurance));
            initialState.NextState(client);

            var expectedContent = new StringContent("endurance=" + initialEndurance);
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
            var initialEntry = CreateRoundEntry(3);
            var newEntry = CreateRoundEntry(1);

            var stubEndpoint = new StubEndpoint(request => CreateResponseWithEntry(newEntry));
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(initialEntry), ApplicationStateInfo.WithEndurance(3));
            var newState = initialState.NextState(client);

            Assert.IsInstanceOf(typeof(ResolvingEncounter), newState);
        }

        [Test]
        public void NewStateShouldContainRevisedEnduranceAsSpecifiedInResponse()
        {
            const int initialEndurance = 3;
            const int newEndurance = 1;

            var initialEntry = CreateRoundEntry(initialEndurance);
            var newEntry = CreateRoundEntry(newEndurance);

            var stubEndpoint = new StubEndpoint(request => CreateResponseWithEntry(newEntry));
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(initialEntry), ApplicationStateInfo.WithEndurance(initialEndurance));
            var newState = initialState.NextState(client);

            Assert.AreEqual(newEndurance, newState.ApplicationStateInfo.Endurance);
        }

        [Test]
        public void IfResponseToSubmittingFormIsNotAtomEntryShouldReturnErrorState()
        {
            var entry = CreateRoundEntry(3);
            
            var stubEndpoint = new StubEndpoint(request => CreateHtmlResponse());
            var client = AtomClient.CreateWithChannel(stubEndpoint);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(entry), ApplicationStateInfo.WithEndurance(5));
            var newState = initialState.NextState(client);

            Assert.IsInstanceOf(typeof(Error), newState);
        }

        [Test]
        public void WhenEnduranceIsZeroShouldReturnDefeatedApplicationState()
        {
            const int endurance = 0;

            var entry = CreateRoundEntry(endurance);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(entry), ApplicationStateInfo.WithEndurance(endurance));
            var newState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof(Defeated), newState);
        }

        [Test]
        public void WhenEnduranceIsLessThanZeroShouldReturnDefeatedApplicationState()
        {
            const int endurance = -1;

            var entry = CreateRoundEntry(endurance);

            var initialState = new ResolvingEncounter(CreateResponseWithEntry(entry), ApplicationStateInfo.WithEndurance(endurance));
            var newState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof(Defeated), newState);
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

        private static HttpResponseMessage CreateHtmlResponse()
        {
            var currentResponse = new HttpResponseMessage { Content = new StringContent("") };
            currentResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return currentResponse;
        }
    }
}