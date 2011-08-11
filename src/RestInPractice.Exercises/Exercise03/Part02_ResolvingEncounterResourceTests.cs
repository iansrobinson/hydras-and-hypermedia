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
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part02_ResolvingEncounterResourceTests
    {
        private static readonly Encounter Encounter = Monsters.NewInstance().Get(1);
        private static readonly KeyValuePair<string, string> ClientEndurance = new KeyValuePair<string, string>("endurance", "10");
        private const string RequestUri = "http://localhost:8081/encounters/1";
        private const string InvalidEncounterId = "999";

        [Test]
        public void ShouldReturn201Created()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void ShouldReturnLocationHeaderWithUriOfNewlyCreatedResource()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));

            Assert.AreEqual(new Uri("http://localhost:8081/encounters/1/round/2"), response.Headers.Location);
        }

        [Test]
        public void ShouldReturn404NotFoundIfPostingToEncounterThatDoesNotExist()
        {
            try
            {
                var resource = CreateResourceUnderTest();
                resource.Post(InvalidEncounterId, CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
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
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));

            Assert.AreEqual(AtomMediaType.Entry, response.Content.Headers.ContentType);
        }

        [Test]
        public void ResponseContentShouldBeSyndicationItem()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationItem), item);
        }

        [Test]
        public void ItemShouldContainRoundCategory()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.IsTrue(item.Categories.Contains(new SyndicationCategory("round"), CategoryComparer.Instance));
        }

        [Test]
        public void ItemIdShouldBeTagUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            
            Assert.AreEqual("tag:restinpractice.com,2011-09-05:/encounters/1/round/2", item.Id);
        }

        [Test]
        public void ItemShouldHaveASelfLink()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            var selfLink = item.Links.First(l => l.RelationshipType.Equals("self"));

            Assert.AreEqual(new Uri("http://localhost:8081/encounters/1/round/2"), selfLink.Uri);
        }

        [Test]
        public void ItemTitleShouldBeRound2()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            
            Assert.AreEqual("Round 2", item.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldIndicateRemainingMonsterEndurance()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual("The " + Encounter.Title + " has 6 Endurance Points", item.Summary.Text);
        }

        [Test]
        public void ItemContentShouldContainXhtml()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual("xhtml", item.Content.Type);
        }

        [Test]
        public void ItemContentShouldIncludeFormContainingRemainingClientEndurance()
        {
            const string @expectedXhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""POST"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""endurance"" value=""9"" />
    <input type=""submit"" value=""Submit"" />
  </form>
</div>";
            
            var resource = CreateResourceUnderTest();
            var response = resource.Post("1", CreateRequest(CreateFormUrlEncodedContent(ClientEndurance)));
            var item = response.Content.ReadAsOrDefault();
            var xhtml = (TextSyndicationContent)item.Content;

            Assert.AreEqual(expectedXhtml, xhtml.Text);
        }

        private static EncounterResource CreateResourceUnderTest()
        {
            return new EncounterResource(Monsters.NewInstance());
        }

        private static HttpRequestMessage CreateRequest(HttpContent content)
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = new Uri(RequestUri), Content = content};
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return request;
        }

        private static HttpContent CreateFormUrlEncodedContent(params KeyValuePair<string, string>[] parameters)
        {
            return new FormUrlEncodedContent(parameters);
        }
    }
}