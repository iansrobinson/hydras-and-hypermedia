using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<Uri> history;

        public Exploring(HttpResponseMessage currentResponse) : this(currentResponse, new Uri[] {})
        {
        }

        public Exploring(HttpResponseMessage currentResponse, IEnumerable<Uri> history)
        {
            this.currentResponse = currentResponse;
            this.history = new List<Uri>(history).AsReadOnly();
        }

        public IApplicationState NextState(HttpClient client)
        {
            var entry = currentResponse.Content.ReadAsObject<SyndicationItemFormatter>(AtomMediaType.Formatter).Item;
            var exitLink = GetExitLink(entry, history, "north", "east", "west", "south");

            var newResponse = client.Get(exitLink.Uri);
            var newHistory = history.Contains(exitLink.Uri) ? history : history.Concat(new[] {exitLink.Uri});

            return new Exploring(newResponse, newHistory);
        }

        private static SyndicationLink GetExitLink(SyndicationItem entry, IEnumerable<Uri> history, params string[] rels)
        {
            var exitLink = rels
                .Select(rel => entry.Links.FirstOrDefault(IsUnvisitedExit(rel, history)))
                .FirstOrDefault(link => link != null);

            if (exitLink == null)
            {
                exitLink = rels
                    .Reverse()
                    .Select(rel => entry.Links.FirstOrDefault(IsUnvisitedExit(rel, new Uri[] {})))
                    .FirstOrDefault(link => link != null);
            }

            return exitLink;
        }

        private static Func<SyndicationLink, bool> IsUnvisitedExit(string rel, IEnumerable<Uri> history)
        {
            return l => l.RelationshipType.Equals(rel) && !history.Contains(l.Uri);
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
        }

        public IEnumerable<Uri> History
        {
            get { return history; }
        }
    }
}