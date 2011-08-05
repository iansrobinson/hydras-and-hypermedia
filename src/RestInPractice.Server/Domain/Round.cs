namespace RestInPractice.Server.Domain
{
    public class Round
    {
        private readonly int id;
        private readonly int endurance;

        public Round(int id, int endurance)
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