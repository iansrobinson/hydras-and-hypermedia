using RestInPractice.Server.Domain;

namespace RestInPractice.Exercises.Helpers
{
    public static class Monsters
    {
        public static Repository<Encounter> Instance
        {
            get
            {
                return new Repository<Encounter>(
                    new Encounter(1, "Minotaur", "A bull-headed monster lumbers from the shadows and swings at you with his axe.", 4, 1, 8));
            }
        }
    }
}