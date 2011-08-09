using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part01_RoomResourceTests
    {
        private static readonly Room Room = Maze.NewInstance().Get(1);
        private const string RequestUri = "http://localhost:8081/rooms/1";
        private const string InvalidRoomId = "999";

        [Test]
        public void ShouldReturn200Ok()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheable()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void ResponseShouldBeCacheableFor10Seconds()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.AreEqual(new TimeSpan(0, 0, 0, 10), response.Headers.CacheControl.MaxAge);
        }

        [Test]
        public void ResponseContentTypeShouldBeApplicationAtomPlusXml()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());

            Assert.AreEqual(AtomMediaType.Value, response.Content.Headers.ContentType);
        }

        [Test]
        public void BodyShouldBeSyndicationItem()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationItem), item);
        }

        [Test]
        public void ItemIdShouldBeTagUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual("tag:restinpractice.com,2011-09-05:/rooms/1", item.Id);
        }

        [Test]
        public void ItemTitleShouldReturnRoomTitle()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Room.Title, item.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldReturnRoomDescription()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Room.Description, item.Summary.Text);
        }

        [Test]
        public void ItemAuthorShouldReturnSystemAdminDetails()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();
            var author = item.Authors.First();

            Assert.AreEqual("Dungeon Master", author.Name);
            Assert.AreEqual("dungeon.master@restinpractice.com", author.Email);
        }

        [Test]
        public void ItemShouldIncludeBaseUri()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(new Uri("http://localhost:8081/"), item.BaseUri);
        }

        [Test]
        public void ItemShouldIncludeLinkToNorth()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            var link = item.Links.First(l => l.RelationshipType.Equals("north"));

            //See Maze class for layout of the maze. Room 4 is north of room 1.
            Assert.AreEqual(new Uri("/rooms/4", UriKind.Relative), link.Uri);
        }

        [Test]
        public void ItemShouldIncludeLinkToEast()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            var link = item.Links.First(l => l.RelationshipType.Equals("east"));

            //See Maze class for layout of the maze. Room 2 is east of room 1.
            Assert.AreEqual(new Uri("/rooms/2", UriKind.Relative), link.Uri);
        }

        [Test]
        public void ItemShouldIncludeLinkToWest()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            var link = item.Links.First(l => l.RelationshipType.Equals("west"));

            //See Maze class for layout of the maze. Room 3 is west of room 1.
            Assert.AreEqual(new Uri("/rooms/3", UriKind.Relative), link.Uri);
        }

        [Test]
        public void ItemShouldNotIncludeLinkToSouth()
        {
            var resource = CreateResourceUnderTest();
            var response = resource.Get("1", CreateRequest());
            var item = response.Content.ReadAsOrDefault();

            Assert.IsNull(item.Links.FirstOrDefault(l => l.RelationshipType.Equals("south")));
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
            return new RoomResource(Maze.NewInstance(), new Repository<Encounter>());
        }

        private static HttpRequestMessage CreateRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, RequestUri);
        }
    }
}