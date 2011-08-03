namespace RestInPractice.Server.Domain
{
    public class EncounterResult
    {
        private readonly int clientEndurance;
        private readonly Outcome outcome;

        public EncounterResult(int clientEndurance, Outcome outcome)
        {
            this.clientEndurance = clientEndurance;
            this.outcome = outcome;
        }

        public int ClientEndurance
        {
            get { return clientEndurance; }
        }

        public Outcome Outcome
        {
            get { return outcome; }
        }
    }
}