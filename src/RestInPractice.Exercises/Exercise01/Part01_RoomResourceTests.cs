using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Formatters;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part01_RoomResourceTests
    {
        private static readonly Room Room = Rooms.Instance.Get(1);
        private const string InvalidRoomId = "999";

        [Test]
        public void ShouldReturn200Ok()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheable()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void ResponseShouldBeCacheableFor10Seconds()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual(new TimeSpan(0, 0, 0, 10), response.Headers.CacheControl.MaxAge);
        }

        [Test]
        public void ResponseContentTypeShouldBeApplicationAtomPlusXml()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual(AtomMediaType.Value, response.Content.Headers.ContentType.MediaType);
        }

        [Test]
        public void BodyShouldBeSyndicationItemFormatter()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationItemFormatter), body);
        }

        [Test]
        public void ItemTitleShouldReturnRoomTitle()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            Assert.AreEqual(Room.Title, body.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldReturnRoomDescription()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            Assert.AreEqual(Room.Description, body.Summary.Text);
        }

        [Test]
        public void ItemAuthorShouldReturnSystemAdminDetails()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;
            var author = body.Authors.First();

            Assert.AreEqual("Dungeon Master", author.Name);
            Assert.AreEqual("dungeon.master@restinpractice.com", author.Email);
        }

        [Test]
        public void ItemShouldIncludeBaseUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            Assert.AreEqual(new Uri("http://localhost/"), body.BaseUri);
        }

        [Test]
        public void ItemShouldIncludeLinkToNorth()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            var link = body.Links.First(l => l.RelationshipType.Equals("north"));
            
            Assert.AreEqual(new Uri("/rooms/2", UriKind.Relative),link.Uri);
        }

        [Test]
        public void ItemShouldIncludeLinkToEast()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            var link = body.Links.First(l => l.RelationshipType.Equals("east"));

            Assert.AreEqual(new Uri("/rooms/3", UriKind.Relative), link.Uri);
        }

        [Test]
        public void ItemShouldIncludeLinkToWest()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            var link = body.Links.First(l => l.RelationshipType.Equals("west"));

            Assert.AreEqual(new Uri("/rooms/4", UriKind.Relative), link.Uri);
        }

        [Test]
        public void ItemShouldNotIncludeLinkToSouth()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            Assert.IsNull(body.Links.FirstOrDefault(l => l.RelationshipType.Equals("south")));
        }

        [Test]
        public void ItemIdShouldBeTagUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault().Item;

            Assert.AreEqual(body.Id, "tag:restinpractice.com,2011-09-05:/rooms/1");
        }

        [Test]
        public void ShouldReturn404NotFoundIfRoomDoesNotExist()
        {
            try
            {
                var resource = CreateResourceUnderTest();
                resource.Get(InvalidRoomId, new HttpRequestMessage());
                Assert.Fail("Expected HttpResponseException");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        private static RoomResource CreateResourceUnderTest()
        {
            return new RoomResource(Rooms.Instance);
        }
    }
}