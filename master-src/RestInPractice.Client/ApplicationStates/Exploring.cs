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
            var exitLink = GetExitLink(entry, "north", "east", "west", "south");
               
            return new Exploring(client.Get(exitLink.Uri));
        }

        private SyndicationLink GetExitLink(SyndicationItem entry, params string[] rels)
        {
            return rels.Select(rel => entry.Links.FirstOrDefault(l => l.RelationshipType.Equals(rel))).FirstOrDefault(exitLink => exitLink != null);
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
        }
    }
}