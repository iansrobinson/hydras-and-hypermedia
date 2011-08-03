using System.Collections.Generic;
using System.Linq;

namespace RestInPractice.Server.Domain
{
    public class Encounter
    {
        private readonly Dictionary<int, Outcome> outcomes;

        public Encounter(int initialEndurance)
        {
            outcomes = new Dictionary<int, Outcome> {{1, new Outcome(1, initialEndurance)}};
        }

        public EncounterResult Action(int clientEndurance)
        {
            var outcome = new Outcome(outcomes.Count + 1, GetAllOutcomes().Last().Endurance - 2);
            outcomes.Add(outcome.Id, outcome);
            return new EncounterResult(clientEndurance - 1, outcome);
        }

        public Outcome GetOutcome(int id)
        {
            return outcomes[id];
        }

        public IEnumerable<Outcome> GetAllOutcomes()
        {
            return outcomes.Values;
        }

        public bool IsResolved
        {
            get { return GetAllOutcomes().Last().Endurance <= 0; }
        }
    }
}