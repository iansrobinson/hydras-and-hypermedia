using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.Client.Comparers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part02_ResolvingEncounterResourceTests
    {
        private static readonly KeyValuePair<string, string> ClientEndurance = new KeyValuePair<string, string>("endurance", "10");
        private const string InvalidEncounterId = "999";

        [Test]
        public void ShouldReturn201Created()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void ShouldReturnLocationHeaderWithUriOfNewlyCreatedResource()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));

            var expectedUri = new Uri(string.Format("http://localhost:8081/encounters/{0}/round/{1}", encounter.Id, encounter.GetAllRounds().Last().Id));

            Assert.AreEqual(expectedUri, response.Headers.Location);
        }

        [Test]
        public void ShouldReturn404NotFoundIfPostingToEncounterThatDoesNotExist()
        {
            try
            {
                var encounter = CreateEncounter();
                var resource = CreateEncounterResource(encounter);
                resource.Post(InvalidEncounterId, CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
                Assert.Fail("Expected 404 Not Found");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        [Test]
        public void ResponseShouldIncludeAtomEntryContentTypeHeader()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));

            Assert.AreEqual(AtomMediaType.Entry, response.Content.Headers.ContentType);
        }

        [Test]
        public void ResponseContentShouldBeSyndicationItem()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationItem), item);
        }

        [Test]
        public void ItemShouldContainRoundCategory()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.IsTrue(item.Categories.Contains(new SyndicationCategory("round"), CategoryComparer.Instance));
        }

        [Test]
        public void ItemIdShouldBeTagUri()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            var expectedItemId = string.Format("tag:restinpractice.com,2011-09-05:/encounters/{0}/round/{1}", encounter.Id, encounter.GetAllRounds().Last().Id);

            Assert.AreEqual(expectedItemId, item.Id);
        }

        [Test]
        public void ItemShouldHaveSelfLink()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            var selfLink = item.Links.First(l => l.RelationshipType.Equals("self"));

            var expectedUri = new Uri(string.Format("http://localhost:8081/encounters/{0}/round/{1}", encounter.Id, encounter.GetAllRounds().Last().Id));

            Assert.AreEqual(expectedUri, selfLink.Uri);
        }

        [Test]
        public void ItemTitleShouldRepresentCurrentRound()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            var expectedTitle = "Round " + encounter.GetAllRounds().Last().Id;
            
            Assert.AreEqual(expectedTitle, item.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldIndicateRemainingMonsterEndurance()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            var expectedSummary = string.Format("The {0} has {1} Endurance Points", encounter.Title, encounter.GetAllRounds().Last().Endurance);
            
            Assert.AreEqual(expectedSummary, item.Summary.Text);
        }

        [Test]
        public void ItemContentShouldContainXhtml()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual("xhtml", item.Content.Type);
        }

        [Test]
        public void ItemContentShouldIncludeFormContainingRemainingClientEndurance()
        {
            var encounter = CreateEncounter();
            var resource = CreateEncounterResource(encounter);
            var response = resource.Post(encounter.Id.ToString(), CreateRequest(encounter.Id, CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            var xhtml = (TextSyndicationContent) item.Content;

            var @expectedXhtml = string.Format(@"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/{0}"" method=""POST"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""endurance"" value=""{1}"" />
    <input type=""submit"" value=""Submit"" />
  </form>
</div>", encounter.Id, int.Parse(ClientEndurance.Value) - 1);

            Assert.AreEqual(expectedXhtml, xhtml.Text);
        }

        private static EncounterResource CreateEncounterResource(Encounter encounter)
        {
            return new EncounterResource(new Repository<Encounter>(encounter));
        }

        private static HttpRequestMessage CreateRequest(int encounterId, HttpContent content)
        {
            var requestUri = new Uri("http://localhost:8081/encounters/" + encounterId);
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = requestUri, Content = content};
            return request;
        }

        private static HttpContent CreateFormUrlEncodedContent(params KeyValuePair<string, string>[] parameters)
        {
            var content = new FormUrlEncodedContent(parameters);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return content;
        }

        private static Encounter CreateEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 8);
        }
    }
}