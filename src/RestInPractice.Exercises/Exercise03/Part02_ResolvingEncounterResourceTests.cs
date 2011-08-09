using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using RestInPractice.Exercises.Helpers;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part02_ResolvingEncounterResourceTests
    {
        private static readonly Encounter Encounter = Monsters.NewInstance().Get(1);
        private const string RequestUri = "http://localhost:8081/encounters/1";

        [Test]
        public void ShouldReturn201Created()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(new FormUrlEncodedContent(new[] {new KeyValuePair<string, string>("endurance", "10"),})));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void ShouldReturnLocationHeaderWithUriOfNewlyCreatedResource()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("endurance", "10"), })));

            Assert.AreEqual(new Uri("http://localhost:8081/encounters/1/round/2"), response.Headers.Location);
        }

        private static EncounterResource CreateResourceUnderTest()
        {
            return new EncounterResource(Monsters.NewInstance());
        }

        private static HttpRequestMessage CreateRequest(FormUrlEncodedContent content)
        {
            return new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = new Uri(RequestUri), Content = content};
        }
    }
}