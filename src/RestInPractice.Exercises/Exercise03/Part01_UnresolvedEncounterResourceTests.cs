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
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part01_UnresolvedEncounterResourceTests
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:8081/");

        [Test]
        public void ShouldReturn200Ok()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldIndicateItMustBeRevalidatedWithOriginServer()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            Assert.IsTrue(response.Headers.CacheControl.NoCache);
        }

        [Test]
        public void ResponseShouldIndicateItMustNotBeStoredByCaches()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            Assert.IsTrue(response.Headers.CacheControl.NoStore);
        }

        [Test]
        public void ResponseShouldIncludeAtomFeedContentTypeHeader()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            Assert.AreEqual(AtomMediaType.Feed, response.Content.Headers.ContentType);
        }

        [Test]
        public void ResponseContentShouldBeSyndicationFeed()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationFeed), feed);
        }

        [Test]
        public void FeedShouldContainEncounterCategory()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsTrue(feed.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance));
        }

        [Test]
        public void FeedIdShouldBeTagUri()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            var expectedId = string.Format("tag:restinpractice.com,2011-09-05:/encounters/{0}", encounter.Id);

            Assert.AreEqual(expectedId, feed.Id);
        }

        [Test]
        public void FeedTitleShouldMatchEncounterTitle()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(encounter.Title, feed.Title.Text);
        }

        [Test]
        public void FeedDescriptionShouldMatchEncounterDescription()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(encounter.Description, feed.Description.Text);
        }

        [Test]
        public void FeedAuthorShouldReturnSystemAdminDetails()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();
            var author = feed.Authors.First();

            Assert.AreEqual("Dungeon Master", author.Name);
            Assert.AreEqual("dungeon.master@restinpractice.com", author.Email);
        }

        [Test]
        public void FeedShouldIncludeBaseUri()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();


            Assert.AreEqual(BaseUri, feed.BaseUri);
        }

        [Test]
        public void FeedShouldIncludeAFleeLink()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();
            var link = feed.Links.First(l => l.RelationshipType.Equals("flee"));

            var expectedUri = new Uri(string.Format("/rooms/{0}", encounter.FleeRoomId), UriKind.Relative);

            Assert.AreEqual(expectedUri, link.Uri);
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
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.IsTrue(feed.ElementExtensions.Contains(
                new SyndicationElementExtension(XmlReader.Create(new StringReader(expectedXhtml))),
                SyndicationElementExtensionComparer.Instance));
        }

        [Test]
        public void FeedShouldIncludeAnItemForEachRound()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(numberOfRounds, feed.Items.Count());
        }

        [Test]
        public void ItemTitlesRepresentRoundsWithMostRecentRoundFirst()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            for (var i = 0; i < numberOfRounds; i++ )
            {
                var expectedTitle = "Round " + (numberOfRounds - i);
                Assert.AreEqual(expectedTitle, feed.Items.ElementAt(i).Title.Text);
            }
        }

        [Test]
        public void EachItemIdShouldBeTagUri()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            for (var i = 0; i < numberOfRounds; i++)
            {
                var expectedId = string.Format("tag:restinpractice.com,2011-09-05:/encounters/{0}/round/{1}", encounter.Id, numberOfRounds - i);
                Assert.AreEqual(expectedId, feed.Items.ElementAt(i).Id);
            } 
        }

        [Test]
        public void ItemIdShouldHaveASelfLink()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();
            
            for (var i = 0; i < numberOfRounds; i++)
            {
                var item = feed.Items.ElementAt(i);
                var selfLink = item.Links.First(l => l.RelationshipType.Equals("self"));

                var expectedUri = new Uri(string.Format("http://localhost:8081/encounters/{0}/round/{1}", encounter.Id, numberOfRounds - i));
                Assert.AreEqual(expectedUri, selfLink.Uri);
            }    
        }

        [Test]
        public void EachItemSummaryShouldDescribeMonsterEnduranceForThatRound()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            for (var i = 0; i < numberOfRounds; i++)
            {
                var item = feed.Items.ElementAt(i);
                var expectedSummary = string.Format("The {0} has {1} Endurance Points", encounter.Title, encounter.GetRound(numberOfRounds - i).Endurance);

                Assert.AreEqual(expectedSummary, item.Summary.Text);
            }   
        }

        [Test]
        public void EachItemShouldContainRoundCategory()
        {
            const int numberOfRounds = 3;
            var encounter = CreateEncounter(numberOfRounds);

            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));
            var feed = response.Content.ReadAsOrDefault();

            for (var i = 0; i < numberOfRounds; i++)
            {
                var item = feed.Items.ElementAt(i);
                Assert.IsTrue(item.Categories.Contains(new SyndicationCategory("round"), CategoryComparer.Instance));
            }
        }

        [Test]
        public void ShouldReturn404NotFoundWhenEncounterDoesNotExist()
        {
            const int invalidEncounterId = 999;

            try
            {
                var resource = CreateEncounterResource(CreateEncounter());
                resource.Get(invalidEncounterId.ToString(), CreateRequest(invalidEncounterId));
                Assert.Fail("Expected 404 Not Found");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        private static EncounterResource CreateEncounterResource(Encounter encounter)
        {
            return new EncounterResource(new Repository<Encounter>(encounter));
        }

        private static HttpRequestMessage CreateRequest(int encounterId)
        {
            var requestUri = new Uri(BaseUri, "/encounters/" + encounterId);
            return new HttpRequestMessage(HttpMethod.Get, requestUri);
        }

        private static Encounter CreateEncounter(int numberOfRounds = 1)
        {
            var encounter = new Encounter(1, "Monster", "Encounter description", 2, 1, 8);
            for (var i = 0; i < numberOfRounds - 1; i++)
            {
                encounter.Action(10);
            }
            return encounter;
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