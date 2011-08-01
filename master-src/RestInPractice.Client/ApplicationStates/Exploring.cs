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
            var northLink = entry.Links.First(l => l.RelationshipType.Equals("north"));
            return new Exploring(client.Get(northLink.Uri));
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
        }
    }
}