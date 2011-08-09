using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.Client.Comparers;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part01_UnresolvedEncounterResourceTests
    {
        private static readonly Encounter Encounter = Monsters.NewInstance().Get(1);
        private const string RequestUri = "http://localhost:8081/encounters/1";
        private const string InvalidEncounterId = "999";

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
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationFeed), feed);
        }

        [Test]
        public void FeedShouldContainEncounterCategory()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsTrue(feed.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance));
        }

        [Test]
        public void FeedIdShouldBeTagUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual("tag:restinpractice.com,2011-09-05:/encounters/1", feed.Id);
        }

        [Test]
        public void FeedTitleShouldMatchEncounterTitle()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Encounter.Title, feed.Title.Text);
        }

        [Test]
        public void FeedDescriptionShouldMatchEncounterDescription()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Encounter.Description, feed.Description.Text);
        }

        [Test]
        public void FeedAuthorShouldReturnSystemAdminDetails()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();
            var author = feed.Authors.First();

            Assert.AreEqual("Dungeon Master", author.Name);
            Assert.AreEqual("dungeon.master@restinpractice.com", author.Email);
        }

        [Test]
        public void FeedShouldIncludeBaseUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(new Uri("http://localhost:8081/"), feed.BaseUri);
        }

        [Test]
        public void FeedShouldIncludeAFleeLink()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            var link = feed.Links.First(l => l.RelationshipType.Equals("flee"));

            Assert.AreEqual(new Uri("/rooms/1", UriKind.Relative), link.Uri);
        }

        [Test]
        public void FeedShouldIncludeAnXhtmlForm()
        {
            const string @expectedXhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""POST"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""endurance""></input>
    <input type=""submit"" value=""Submit""></input>
  </form>
</div>";
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsTrue(feed.ElementExtensions.Contains(
                new SyndicationElementExtension(XmlReader.Create(new StringReader(expectedXhtml))),
                SyndicationElementExtensionComparer.Instance));
        }

        [Test]
        public void FeedShouldIncludeAnItem()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(1, feed.Items.Count());
        }

        [Test]
        public void ItemTitleShouldBeRound1()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();
            var item = feed.Items.First();

            Assert.AreEqual("Round 1", item.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldDescribeMonsterEndurance()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();
            var item = feed.Items.First();

            Assert.AreEqual("The Minotaur has 8 Endurance Points", item.Summary.Text);
        }

        [Test]
        public void ItemShouldContainRoundCategory()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var feed = response.Content.ReadAsOrDefault();
            var item = feed.Items.First();

            Assert.IsTrue(item.Categories.Contains(new SyndicationCategory("round"), CategoryComparer.Instance));
        }

        [Test]
        public void ShouldReturn404NotFoundWhenEncounterDoesNotExist()
        {
            try
            {
                var resource = CreateResourceUnderTest();
                resource.Get(InvalidEncounterId, CreateRequest());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        private static EncounterResource CreateResourceUnderTest()
        {
            return new EncounterResource(Monsters.NewInstance());
        }

        private static HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, RequestUri);
        }

        private class SyndicationElementExtensionComparer : IEqualityComparer<SyndicationElementExtension>
        {
            public static readonly IEqualityComparer<SyndicationElementExtension> Instance = new SyndicationElementExtensionComparer();

            private SyndicationElementExtensionComparer()
            {
            }

            public bool Equals(SyndicationElementExtension x, SyndicationElementExtension y)
            {
                return x.GetReader().ReadContentAsString().Equals(y.GetReader().ReadContentAsString());
            }

            public int GetHashCode(SyndicationElementExtension obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}