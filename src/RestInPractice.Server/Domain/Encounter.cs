using System;
using System.Collections.Generic;
using System.Linq;

namespace RestInPractice.Server.Domain
{
    public class Encounter : IIdentifiable
    {
        private readonly int id;
        private readonly string title;
        private readonly string description;
        private readonly int guardedRoomId;
        private readonly int fleeRoomId;
        private readonly Dictionary<int, Round> rounds;

        public Encounter(int id, string title, string description, int guardedRoomId, int fleeRoomId, int initialEndurance)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.guardedRoomId = guardedRoomId;
            this.fleeRoomId = fleeRoomId;
            rounds = new Dictionary<int, Round> {{1, new Round(1, initialEndurance)}};
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
            
            var round = new Round(rounds.Count + 1, GetAllRounds().Last().Endurance - 2);
            rounds.Add(round.Id, round);
            return new EncounterResult(clientEndurance - 1, round);
        }

        public Round GetRound(int encounterId)
        {
            return rounds[encounterId];
        }

        public IEnumerable<Round> GetAllRounds()
        {
            return rounds.Values;
        }

        public bool IsResolved
        {
            get { return GetAllRounds().Last().Endurance <= 0; }
        }
    }
}