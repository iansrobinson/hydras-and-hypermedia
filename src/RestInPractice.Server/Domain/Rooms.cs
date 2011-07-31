using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Rooms
    {
        public static readonly Rooms Instance = new Rooms(
            new Room(1, "Entrance", "You descend a rope into a rubble-strewn hall. The air is cold and dank.", Exit.North(2), Exit.East(3), Exit.West(4)),
            new Room(2, "Steps", "A flight of steep steps lead down into the darkness. A blast of warm, fetid air issues from below, followed by an inhuman shriek.", Exit.South(1)),
            new Room(3, "Dungeon Cell", "The passage emerges into a narrow cell.", Exit.West(1)),
            new Room(4, "Dead End", "The passage narrows and ends in a rough stone wall.", Exit.East(1))
            );
        
        private readonly IDictionary<int, Room> roomsDictionary;

        private Rooms(params Room[] rooms)
        {
            roomsDictionary = new Dictionary<int, Room>(rooms.Length);

            foreach (var room in rooms)
            {
                roomsDictionary.Add(room.Id, room);
            }
        }

        public Room Get(int roomId)
        {
            return roomsDictionary[roomId];
        }
    }
}