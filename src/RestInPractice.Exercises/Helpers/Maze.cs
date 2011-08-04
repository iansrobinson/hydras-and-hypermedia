using RestInPractice.Server.Domain;

namespace RestInPractice.Exercises.Helpers
{
    //      9   7---6
    //      |   |   |
    // 10---8---4---5
    //          |
    //      3---1---2
    public static class Maze
    {
        public static Repository<Room> Instance
        {
            get
            {
                return new Repository<Room>(
                    new Room(1, "Entrance", "Maze entrance.", Exit.North(4), Exit.East(2), Exit.West(3)),
                    new Room(2, "Room 2", "Room 2 description", Exit.West(1)),
                    new Room(3, "Room 3", "Room 3 description", Exit.East(1)),
                    new Room(4, "Room 4", "Room 4 description", Exit.North(7), Exit.South(1), Exit.East(5), Exit.West(8)),
                    new Room(5, "Room 5", "Room 5 description", Exit.North(6), Exit.West(4)),
                    new Room(6, "Room 6", "Room 6 description", Exit.South(5), Exit.West(7)),
                    new Room(7, "Room 7", "Room 7 description", Exit.South(4), Exit.East(6)),
                    new Room(8, "Room 8", "Room 8 description", Exit.North(9), Exit.East(4), Exit.West(10)),
                    new Room(9, "Room 9", "Room 9 description", Exit.South(8)),
                    new Room(10, "Exit", "Maze exit", Exit.East(8))
                    );
            }
        }
    }
}