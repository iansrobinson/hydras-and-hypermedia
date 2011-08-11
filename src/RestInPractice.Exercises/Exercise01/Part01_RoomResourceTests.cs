using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part01_RoomResourceTests
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:8081/");

        [Test]
        public void ShouldReturn200Ok()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseShouldBePublicallyCacheable()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void ResponseShouldBeCacheableFor10Seconds()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.AreEqual(new TimeSpan(0, 0, 0, 10), response.Headers.CacheControl.MaxAge);
        }

        [Test]
        public void ResponseContentTypeShouldBeApplicationAtomPlusXml()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.AreEqual(AtomMediaType.Value, response.Content.Headers.ContentType);
        }

        [Test]
        public void BodyShouldBeSyndicationItem()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            Assert.IsInstanceOf(typeof (SyndicationItem), item);
        }

        [Test]
        public void ItemIdShouldBeTagUri()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            var expectedId = string.Format("tag:restinpractice.com,2011-09-05:/rooms/{0}", room.Id);

            Assert.AreEqual(expectedId, item.Id);
        }

        [Test]
        public void ItemTitleShouldReturnRoomTitle()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(room.Title, item.Title.Text);
        }

        [Test]
        public void ItemSummaryShouldReturnRoomDescription()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(room.Description, item.Summary.Text);
        }

        [Test]
        public void ItemAuthorShouldReturnSystemAdminDetails()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();
            var author = item.Authors.First();

            Assert.AreEqual("Dungeon Master", author.Name);
            Assert.AreEqual("dungeon.master@restinpractice.com", author.Email);
        }

        [Test]
        public void ItemShouldIncludeBaseUri()
        {
            var room = CreateRoom();
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            Assert.AreEqual(BaseUri, item.BaseUri);
        }

        [Test]
        public void ItemShouldIncludeLinksToExits()
        {
            var exits = new[] {Exit.North(2), Exit.South(3), Exit.East(4), Exit.West(5)};

            var room = CreateRoom(exits);
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            foreach (var exit in exits)
            {
                var linkRelationship = exit.Direction.ToString().ToLower();
                var expectedUri = new Uri("/rooms/" + exit.RoomId, UriKind.Relative);

                var link = item.Links.First(l => l.RelationshipType.Equals(linkRelationship));

                Assert.AreEqual(expectedUri, link.Uri);
            }
        }

        [Test]
        public void ItemShouldNotIncludeLinksToExitsThatDoNotExist()
        {
            var exits = new Exit[]{ };

            var room = CreateRoom(exits);
            var resource = CreateRoomResource(room);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));
            var item = response.Content.ReadAsOrDefault();

            var linkRelationships = new[] {"north", "south", "east", "west"};

            foreach (var link in linkRelationships.Select(rel => item.Links.FirstOrDefault(l => l.RelationshipType.Equals(rel))))
            {
                Assert.IsNull(link);
            }
        }

        [Test]
        public void ShouldReturn404NotFoundIfRoomDoesNotExist()
        {
            try
            {
                var resource = CreateRoomResource(CreateRoom());
                resource.Get("999", new HttpRequestMessage());
                Assert.Fail("Expected 404 Not Found");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            }
        }

        private static RoomResource CreateRoomResource(Room room)
        {
            return new RoomResource(new Repository<Room>(room), new Repository<Encounter>());
        }

        private static HttpRequestMessage CreateRequest(int roomId)
        {
            var requestUri = new Uri(BaseUri, "encounters/" + roomId);
            return new HttpRequestMessage(HttpMethod.Get, requestUri);
        }

        private static Room CreateRoom(params Exit[] exits)
        {
            return new Room(1, "Entrance", "Maze entrance.", exits);
        }
    }
}