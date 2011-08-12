using System;
using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Room : IIdentifiable
    {
        private readonly int id;
        private readonly string title;
        private readonly string description;
        private readonly IEnumerable<Exit> exits;
        private readonly int? encounterId;

        public Room(int id, string title, string description, params Exit[] exits) : this(id, title, description, null, exits)
        {
        }

        public Room(int id, string title, string description, int? encounterId, params Exit[] exits)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.encounterId = encounterId;
            this.exits = exits;
        }

        public Encounter GetEncounter(Repository<Encounter> encounterRepository)
        {
            if (!encounterId.HasValue)
            {
                throw new InvalidOperationException("There is no encounter for this room.");
            }
            
            return encounterRepository.Get(encounterId.Value);
        }

        public bool IsGuarded(Repository<Encounter> encounterRepository)
        {
            if (encounterId.HasValue)
            {
                return !GetEncounter(encounterRepository).IsResolved;
            }
            return false;
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

        public IEnumerable<Exit> Exits
        {
            get { return exits; }
        }
    }
}