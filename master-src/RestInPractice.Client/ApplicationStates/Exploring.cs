using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using RestInPractice.Client.Extensions;
using RestInPractice.MediaTypes;

namespace RestInPractice.Client.ApplicationStates
{
    public class Exploring : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;

        public Exploring(HttpResponseMessage currentResponse)
        {
            this.currentResponse = currentResponse;
        }

        public IApplicationState NextState(HttpClient client)
        {
            var entry = currentResponse.Content.ReadAsObject<SyndicationItemFormatter>(AtomMediaType.Formatter).Item;
            var exitLink = entry.Links.FirstOrDefault(l => l.RelationshipType.Equals("north"));
            
            if (exitLink == null)
            {
                exitLink = entry.Links.FirstOrDefault(l => l.RelationshipType.Equals("east"));
            }
            if (exitLink == null)
            {
                exitLink = entry.Links.FirstOrDefault(l => l.RelationshipType.Equals("west"));
            }
            if (exitLink == null)
            {
                exitLink = entry.Links.FirstOrDefault(l => l.RelationshipType.Equals("south"));
            }
               
            return new Exploring(client.Get(exitLink.Uri));
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
        }
    }
}