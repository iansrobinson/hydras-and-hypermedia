using System.Collections.Generic;

namespace RestInPractice.Server.Domain
{
    public class Rooms
    {
        private readonly IDictionary<int, Room> roomsDictionary;

        public Rooms(params Room[] rooms)
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