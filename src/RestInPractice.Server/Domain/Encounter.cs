using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Encounter
    {
        private readonly int initialEndurance;
        private readonly Dictionary<int, Outcome> outcomes;

        public Encounter(int initialEndurance)
        {
            this.initialEndurance = initialEndurance;
            outcomes = new Dictionary<int, Outcome>();
        }

        public EncounterResult Action(int clientEndurance)
        {
            var outcome = new Outcome(outcomes.Count + 1, initialEndurance - 2);
            outcomes.Add(outcome.Id, outcome);
            return new EncounterResult(clientEndurance - 1, outcome);
        }

        public Outcome GetOutcome(int id)
        {
            return outcomes[id];
        }

        public IEnumerable<Outcome> Outcomes
        {
            get { return outcomes.Values; }
        }
    }
}