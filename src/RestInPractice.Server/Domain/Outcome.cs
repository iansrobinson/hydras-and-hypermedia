namespace RestInPractice.Server.Domain
{
    public class Outcome
    {
        private readonly int id;
        private readonly int endurance;

        public Outcome(int id, int endurance)
        {
            this.id = id;
            this.endurance = endurance;
        }

        public int Id
        {
            get { return id; }
        }

        public int Endurance
        {
            get { return endurance; }
        }
    }
}