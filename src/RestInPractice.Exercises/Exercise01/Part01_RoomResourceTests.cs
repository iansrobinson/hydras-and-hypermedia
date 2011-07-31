using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part01_RoomResourceTests
    {
        private static readonly Room Room = new Room(1, "Entrance", "You descend a rope into a rubble-strewn hall. The air is cold and dank.", Exit.North(2), Exit.East(3), Exit.West(4));
        private static readonly Rooms Rooms = new Rooms(Room);
        private const string InvalidRoomId = "999";

        [Test]
        public void ShouldReturn200Ok()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheable()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void ResponseShouldBeCacheableFor10Seconds()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual(new TimeSpan(0, 0, 0, 10), response.Headers.CacheControl.MaxAge);
        }

        [Test]
        public void ResponseContentTypeShouldBeApplicationAtomPlusXml()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());

            Assert.AreEqual("application/atom+xml", response.Content.Headers.ContentType.MediaType);
        }

        [Test]
        public void BodyShouldBeSyndicationFeed()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationFeed), body);
        }

        [Test]
        public void FeedTitleShouldReturnRoomDescription()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Room.Title, body.Title.Text);
        }

        [Test]
        public void FeedDescriptionShouldReturnRoomDescription()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.AreEqual(Room.Description, body.Description.Text);
        }

        [Test]
        public void FeedShouldIncludeBaseUri()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.AreEqual(new Uri("http://localhost/"), body.BaseUri);
        }

        [Test]
        public void FeedShouldIncludeLinkToNorth()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            var link = body.Links.First(l => l.RelationshipType.Equals("north"));
            
            Assert.AreEqual(new Uri("/rooms/2", UriKind.Relative),link.Uri);
        }

        [Test]
        public void FeedShouldIncludeLinkToEast()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            var link = body.Links.First(l => l.RelationshipType.Equals("east"));

            Assert.AreEqual(new Uri("/rooms/3", UriKind.Relative), link.Uri);
        }

        [Test]
        public void FeedShouldIncludeLinkToWest()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            var link = body.Links.First(l => l.RelationshipType.Equals("west"));

            Assert.AreEqual(new Uri("/rooms/4", UriKind.Relative), link.Uri);
        }

        [Test]
        public void FeedShouldNotIncludeLinkToSouth()
        {
            var resource = new RoomResource(Rooms);
            var response = resource.Get("1", new HttpRequestMessage());
            var body = response.Content.ReadAsOrDefault();

            Assert.IsNull(body.Links.FirstOrDefault(l => l.RelationshipType.Equals("south")));
        }

        [Test]
        public void ShouldReturn404NotFoundIfRoomDoesNotExist()
        {
            try
            {
                var resource = new RoomResource(Rooms);
                resource.Get(InvalidRoomId, new HttpRequestMessage());
                Assert.Fail("Expected HttpResponseException");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }
    }
}