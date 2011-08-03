using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Encounter
    {
        private readonly int initialEndurance;
        private readonly List<Outcome> outcomes;

        public Encounter(int initialEndurance)
        {
            this.initialEndurance = initialEndurance;
            outcomes = new List<Outcome>();
        }

        public EncounterResult Action(int clientEndurance)
        {
            var outcome = new Outcome(initialEndurance - 2);
            outcomes.Add(outcome);
            return new EncounterResult(clientEndurance - 1, outcome);
        }

        public IEnumerable<Outcome> Outcomes
        {
            get { return outcomes.AsReadOnly(); }
        }
    }
}