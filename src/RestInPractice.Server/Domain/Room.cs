using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Room
    {
        private readonly int id;
        private readonly string title;
        private readonly string description;
        private readonly IEnumerable<Exit> exits;

        public Room(int id, string title, string description, params Exit[] exits)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.exits = exits;
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