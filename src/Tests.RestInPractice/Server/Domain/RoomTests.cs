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
        }

        [Test]
        public void ShouldIndicatedItIsNotGuardedIfThereIsNoEncounter()
        {
        }

        [Test]
        public void ShouldIndicateItIsGuardedIfThereIsAnUnresolvedEncounter()
        {
        }

        [Test]
        public void ShouldIndicateItIsNotGuardedIfThereIsAResolvedEncounter()
        {
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