﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using NUnit.Framework;
using RestInPractice.Client.Comparers;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part01_EncounterResourceTests
    {
        private static readonly Encounter Encounter = Monsters.Instance.Get(1);
        private const string RequestUri = "http://localhost:8081/encounters/1";

        [Test]
        public void ShouldReturn200Ok()
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

        [Test]
        public void FeedShouldContainEncounterCategory()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var body = response.Content.ReadAsOrDefault();

            Assert.IsTrue(body.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance));
        }

        [Test]
        public void FeedIdShouldBeTagUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var body = response.Content.ReadAsOrDefault();

            Assert.AreEqual("tag:restinpractice.com,2011-09-05:/encounters/1", body.Id);
        }

        [Test]
        public void FeedTitleShouldMatchEncounterTitle()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var body = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Encounter.Title, body.Title.Text);
        }

        private static EncounterResource CreateResourceUnderTest()
        {
            return new EncounterResource(Monsters.Instance);
        }

        private static HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, RequestUri);
        }
    }
}