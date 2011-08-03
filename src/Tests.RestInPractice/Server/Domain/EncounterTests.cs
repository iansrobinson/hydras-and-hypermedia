using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RestInPractice.Server.Domain;

namespace Tests.RestInPractice.Server.Domain
{
    [TestFixture]
    public class EncounterTests
    {
        private const int EncounterEndurance = 10;
        private const int ClientEndurance = 5;
        
        [Test]
        public void ActionShouldReduceEncounterEnduranceByTwo()
        {
            var encounter = new Encounter(EncounterEndurance);
            var result = encounter.Action(ClientEndurance);

            Assert.AreEqual(8, result.Outcome.Endurance);
        }

        [Test]
        public void ActionShouldReduceClientEnduranceByOne()
        {
            var encounter = new Encounter(EncounterEndurance);
            var result = encounter.Action(ClientEndurance);

            Assert.AreEqual(4, result.ClientEndurance);
        }

        [Test]
        public void ShouldAddOutcomeToListOfOutcomes()
        {
            var encounter = new Encounter(EncounterEndurance);
            
            Assert.AreEqual(0, encounter.Outcomes.Count());

            var result = encounter.Action(ClientEndurance);

            Assert.AreEqual(result.Outcome, encounter.Outcomes.First());
        }

        [Test]
        public void OutcomesShouldHaveIncrementingIds()
        {
            var encounter = new Encounter(EncounterEndurance);

            encounter.Action(5);
            encounter.Action(4);
            encounter.Action(3);

            Assert.AreEqual(1, encounter.Outcomes.ElementAt(0).Id);
            Assert.AreEqual(2, encounter.Outcomes.ElementAt(1).Id);
            Assert.AreEqual(3, encounter.Outcomes.ElementAt(2).Id);
        }

        [Test]
        public void ShouldBeAbleToRetrieveOutcomeById()
        {
            var encounter = new Encounter(EncounterEndurance);

            encounter.Action(5);
            var result = encounter.Action(4);

            Assert.AreEqual(result.Outcome, encounter.GetOutcome(result.Outcome.Id));
        }
    }
}
