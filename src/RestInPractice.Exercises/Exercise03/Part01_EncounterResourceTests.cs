using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part01_EncounterResourceTests
    {
        private const string RequestUri = "http://localhost:8081/encounters/1";

        [Test]
        public void ShouldReturn200OK()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldIndicateItMustBeRevalidatedWithOriginServer()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.IsTrue(response.Headers.CacheControl.NoCache);
        }

        [Test]
        public void ResponseShouldIndicateItMustNotBeStoredByCaches()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.IsTrue(response.Headers.CacheControl.NoStore);
        }

        [Test]
        public void ResponseShouldIncludeAtomFeedContentTypeHeader()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.AreEqual(AtomMediaType.Feed, response.Content.Headers.ContentType);
        }

        [Test]
        public void ResponseContentShouldBeSyndicationFeed()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var body = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationFeed), body);
        }

        private static EncounterResource CreateResourceUnderTest()
        {
            return new EncounterResource();
        }

        private static HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, RequestUri);
        }
    }
}