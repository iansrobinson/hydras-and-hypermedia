namespace HydrasAndHypermedia.Server.Domain
{
    public class EncounterResult
    {
        private readonly int clientEndurance;
        private readonly Round round;

        public EncounterResult(int clientEndurance, Round round)
        {
            this.clientEndurance = clientEndurance;
            this.round = round;
        }

        public int ClientEndurance
        {
            get { return clientEndurance; }
        }

        public Round Round
        {
            get { return round; }
        }
    }
}