using System;
using System.Linq;
using NUnit.Framework;
using RestInPractice.Server.Domain;

namespace Tests.RestInPractice.Server.Domain
{
    [TestFixture]
    public class EncounterTests
    {
        private const int EncounterEndurance = 10;
        private const int ClientEndurance = 6;

        [Test]
        public void ActionShouldReduceEncounterEnduranceByTwo()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);
            var firstResult = encounter.Action(ClientEndurance);

            Assert.AreEqual(8, firstResult.Outcome.Endurance);

            var secondResult = encounter.Action(ClientEndurance);

            Assert.AreEqual(6, secondResult.Outcome.Endurance);
        }

        [Test]
        public void ActionShouldReduceClientEnduranceByOne()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);
            var result = encounter.Action(ClientEndurance);

            Assert.AreEqual(5, result.ClientEndurance);
        }

        [Test]
        public void ShouldAddOutcomeToListOfOutcomes()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);

            Assert.AreEqual(1, encounter.GetAllOutcomes().Count());

            var result = encounter.Action(ClientEndurance);

            Assert.AreEqual(2, encounter.GetAllOutcomes().Count());
            Assert.AreEqual(result.Outcome, encounter.GetAllOutcomes().Last());
        }

        [Test]
        public void OutcomesShouldHaveIncrementingIds()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);

            encounter.Action(5);
            encounter.Action(4);
            
            Assert.AreEqual(1, encounter.GetAllOutcomes().ElementAt(0).Id);
            Assert.AreEqual(2, encounter.GetAllOutcomes().ElementAt(1).Id);
            Assert.AreEqual(3, encounter.GetAllOutcomes().ElementAt(2).Id);
        }

        [Test]
        public void ShouldBeAbleToRetrieveOutcomeById()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);

            encounter.Action(5);
            var result = encounter.Action(4);

            Assert.AreEqual(result.Outcome, encounter.GetOutcome(result.Outcome.Id));
        }

        [Test]
        public void ShouldIndicateThatEncounterIsResolvedWhenEnduranceIsExhausted()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);

            for (var counter = 0; counter < 4; counter++)
            {
                encounter.Action(ClientEndurance - counter);
                Assert.IsFalse(encounter.IsResolved);
            }

            encounter.Action(2);
            Assert.IsTrue(encounter.IsResolved);
        }
        
        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Encounter is already resolved.")]
        public void ThrowsExceptionIfTryingToPeformActionAgainstResolvedEncounter()
        {
            var encounter = CreateEncounterUnderTest(0);
            encounter.Action(2);
        }

        [Test]
        [ExpectedException(ExpectedException= typeof (ArgumentException), ExpectedMessage = "Endurance must be greater than zero.\r\nParameter name: clientEndurance")]
        public void ThrowsExceptionIfClientEnduranceIsZero()
        {
            var encounter = CreateEncounterUnderTest(EncounterEndurance);
            encounter.Action(0);
        }

        public static Encounter CreateEncounterUnderTest(int endurance)
        {
            return new Encounter(2, 1, endurance);
        }
    }
}