using System;
using System.Collections.Generic;
using System.Linq;

namespace RestInPractice.Server.Domain
{
    public class Encounter
    {
        private readonly int id;
        private readonly string title;
        private readonly string description;
        private readonly int guardedRoomId;
        private readonly int fleeRoomId;
        private readonly Dictionary<int, Outcome> outcomes;

        public Encounter(int id, string title, string description, int guardedRoomId, int fleeRoomId, int initialEndurance)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.guardedRoomId = guardedRoomId;
            this.fleeRoomId = fleeRoomId;
            outcomes = new Dictionary<int, Outcome> {{1, new Outcome(1, initialEndurance)}};
        }

        public int Id
        {
            get { return id; }
        }

        public string Title
        {
            get { return title; }
        }

        public string Description
        {
            get { return description; }
        }

        public int GuardedRoomId
        {
            get { return guardedRoomId; }
        }

        public int FleeRoomId
        {
            get { return fleeRoomId; }
        }

        public EncounterResult Action(int clientEndurance)
        {
            if (IsResolved)
            {
                throw new InvalidOperationException("Encounter is already resolved.");
            }

            if (clientEndurance == 0)
            {
                throw new ArgumentException("Endurance must be greater than zero.", "clientEndurance");
            }
            
            var outcome = new Outcome(outcomes.Count + 1, GetAllOutcomes().Last().Endurance - 2);
            outcomes.Add(outcome.Id, outcome);
            return new EncounterResult(clientEndurance - 1, outcome);
        }

        public Outcome GetOutcome(int encounterId)
        {
            return outcomes[encounterId];
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