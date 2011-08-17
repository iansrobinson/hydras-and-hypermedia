using HydrasAndHypermedia.Server.Domain;

namespace HydrasAndHypermedia.Exercises.Helpers
{
    public static class Monsters
    {
        public static Repository<Encounter> NewInstance()
        {
            return new Repository<Encounter>(
                new Encounter(1, "Minotaur", "A bull-headed monster lumbers from the shadows and swings at you with his axe.", 4, 1, 8),
                new Encounter(2, "Hydra", "A giant, many-headed serpent rears up and lashes at you with its fanged jaws.", 2, 1, 12));
        }

        public static Repository<Encounter> NullEncounters()
        {
            return new Repository<Encounter>(
                new Encounter(1, "Null encounter", string.Empty, 4, 1, 0),
                new Encounter(2, "Null encounter", string.Empty, 2, 1, 0));
        }
    }
}