using System;
using System.Net;
using System.Net.Http;
using HydrasAndHypermedia.Server.Domain;
using HydrasAndHypermedia.Server.Resources;
using NUnit.Framework;

namespace HydrasAndHypermedia.Exercises.Exercise03
{
    [TestFixture]
    public class Part04_RoomResourceTests
    {
        [Test]
        public void WhenRoomIsGuardedByUnresolvedEncounterResponseShouldBe303SeeOther()
        {
            var encounter = CreateUnresolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);
            var resource = CreateRoomResource(room, encounter);

            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [Test]
        public void WhenRoomIsGuardedByUnresolvedEncounterResponseShouldContainLocationHeaderWithAddressOfEncounter()
        {
            var encounter = CreateUnresolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);
            var resource = CreateRoomResource(room, encounter);
            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            var expectedUri = new Uri(string.Format("http://{0}:8081/encounters/{1}", Environment.MachineName, encounter.Id));

            Assert.AreEqual(expectedUri, response.Headers.Location);
        }

        [Test]
        public void WhenRoomIsGuardedByResolvedEncounterResponseShouldBe200Ok()
        {
            var encounter = CreateResolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);
            var resource = CreateRoomResource(room, encounter);

            var response = resource.Get(room.Id.ToString(), CreateRequest(room.Id));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private static HttpRequestMessage CreateRequest(int roomId)
        {
            var requestUri = new Uri("http://localhost:8081/encounters/" + roomId);
            return new HttpRequestMessage(HttpMethod.Get, requestUri);
        }

        private static RoomResource CreateRoomResource(Room room, Encounter encounter)
        {
            return new RoomResource(new Repository<Room>(room), new Repository<Encounter>(encounter));
        }

        private static Room CreateRoomWithEncounter(int encounterId)
        {
            return new Room(1, "Room", "Room description", encounterId);
        }

        private static Encounter CreateUnresolvedEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 8);
        }

        private static Encounter CreateResolvedEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 0);
        }
    }
}