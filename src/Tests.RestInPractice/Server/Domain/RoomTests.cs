using NUnit.Framework;
using RestInPractice.Server.Domain;

namespace Tests.RestInPractice.Server.Domain
{
    [TestFixture]
    public class RoomTests
    {
        [Test]
        public void ShouldReturnEncounter()
        {
            var encounter = CreateUnresolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);

            var returnedEncounter = room.GetEncounter(new Repository<Encounter>(encounter));

            Assert.AreEqual(encounter, returnedEncounter);
        }

        [Test]
        public void ShouldIndicatedItIsNotGuardedIfThereIsNoEncounter()
        {
            var room = CreateRoomWithoutEncounter();
            Assert.IsFalse(room.IsGuarded(new Repository<Encounter>()));
        }

        [Test]
        public void ShouldIndicateItIsGuardedIfThereIsAnUnresolvedEncounter()
        {
            var encounter = CreateUnresolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);

            Assert.IsTrue(room.IsGuarded(new Repository<Encounter>(encounter)));
        }

        [Test]
        public void ShouldIndicateItIsNotGuardedIfThereIsAResolvedEncounter()
        {
            var encounter = CreateResolvedEncounter();
            var room = CreateRoomWithEncounter(encounter.Id);

            Assert.IsFalse(room.IsGuarded(new Repository<Encounter>(encounter)));
        }

        public static Room CreateRoomWithEncounter(int encounterId)
        {
            return new Room(1, "Room", "Room description", encounterId);
        }

        public static Room CreateRoomWithoutEncounter()
        {
            return new Room(1, "Room", "Room description");
        }

        public static Encounter CreateUnresolvedEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 8);
        }

        public static Encounter CreateResolvedEncounter()
        {
            return new Encounter(1, "Monster", "Encounter description", 2, 1, 0);
        }
    }
}