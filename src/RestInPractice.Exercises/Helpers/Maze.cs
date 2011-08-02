﻿using RestInPractice.Server.Domain;

namespace RestInPractice.Exercises.Helpers
{
    public static class Maze
    {
        public static Rooms Instance
        {
            get
            {
                return new Rooms(
                    new Room(1, "Room 1", "Room 1 description.", Exit.North(4), Exit.East(2), Exit.West(3)),
                    new Room(2, "Room 2", "Room 2 description.", Exit.West(1)),
                    new Room(3, "Room 3", "Room 3 description.", Exit.East(1)),
                    new Room(4, "Room 4", "Room 4 description.", Exit.North(7), Exit.South(1), Exit.East(5), Exit.West(8)),
                    new Room(5, "Room 5", "Room 5 description.", Exit.North(6), Exit.West(4)),
                    new Room(6, "Room 6", "Room 6 description.", Exit.South(5), Exit.West(7)),
                    new Room(7, "Room 7", "Room 7 description.", Exit.South(4), Exit.East(6)),
                    new Room(8, "Room 8", "Room 8 description.", Exit.North(9), Exit.East(4), Exit.West(10)),
                    new Room(9, "Room 9", "Room 9 description.", Exit.South(8)),
                    new Room(10, "Success", "Maze exit", Exit.East(8))
                    );
            }
        }

        public static Rooms ExistingInstance
        {
            get
            {
                return new Rooms(
                    new Room(1, "Entrance", "You descend a rope into a rubble-strewn hall. The air is cold and dank.", Exit.North(2), Exit.East(3), Exit.West(4)),
                    new Room(2, "Steps", "A flight of steep steps lead down into the darkness. A blast of warm, fetid air issues from below, followed by an inhuman shriek.", Exit.South(1)),
                    new Room(3, "Dungeon Cell", "The passage emerges into a narrow cell.", Exit.West(1)),
                    new Room(4, "Dead End", "The passage narrows and ends in a rough stone wall.", Exit.East(1))
                    );
            }
        }
    }
}