using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part03_ResolvedEncounterResourceTests
    {
        [Test]
        public void WhenEncounterIsResolvedFeedDoesNotIncludeAFleeLlink()
        {
            var encounter = CreateResolvedEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            var feed = response.Content.ReadAsOrDefault();
            var link = feed.Links.FirstOrDefault(l => l.RelationshipType.Equals("flee"));

            Assert.IsNull(link);
        }

        [Test]
        public void WhenEncounterIsResolvedFeedDoesNotIncludeAnXhtmlForm()
        {
            var encounter = CreateResolvedEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            var feed = response.Content.ReadAsOrDefault();

            Assert.AreEqual(0, feed.ElementExtensions.Count());
        }

        [Test]
        public void WhenEncounterIsResolvedFeedIncludesAContinueLink()
        {
            var encounter = CreateResolvedEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Get(encounter.Id.ToString(), CreateRequest(encounter.Id));

            var feed = response.Content.ReadAsOrDefault();
            var link = feed.Links.FirstOrDefault(l => l.RelationshipType.Equals("continue"));

            var expectedUri = new Uri(string.Format("/rooms/{0}", encounter.GuardedRoomId), UriKind.Relative);

            Assert.AreEqual(expectedUri, link.Uri);
        }

        private static HttpRequestMessage CreateRequest(int encounterId)
        {
            var requestUri = new Uri("http://localhost:8081/encounters/" + encounterId);
            var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = requestUri };
            return request;
        }

        private static EncounterResource CreateEncounterResource(Encounter encounter)
        {
            return new EncounterResource(new Repository<Encounter>(encounter));
        }

        private static Encounter CreateResolvedEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 0);
        }
    }
}
