namespace RestInPractice.Server.Domain
{
    public class Outcome
    {
        private readonly int endurance;

        public Outcome(int endurance)
        {
            this.endurance = endurance;
        }

        public int Endurance
        {
            get { return endurance; }
        }
    }
}