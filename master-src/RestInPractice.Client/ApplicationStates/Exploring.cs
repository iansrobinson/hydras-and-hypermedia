using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using RestInPractice.Client.Comparers;
using RestInPractice.Client.Extensions;
using RestInPractice.MediaTypes;

namespace RestInPractice.Client.ApplicationStates
{
    public class Exploring : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;
        private readonly ApplicationStateInfo applicationStateInfo;

        public Exploring(HttpResponseMessage currentResponse, ApplicationStateInfo applicationStateInfo)
        {
            this.currentResponse = currentResponse;
            this.applicationStateInfo = applicationStateInfo;
        }

        public IApplicationState NextState(HttpClient client)
        {
            if (currentResponse.Content.Headers.ContentType.Equals(AtomMediaType.Feed))
            {
                var feed = currentResponse.Content.ReadAsObject<SyndicationFeed>(AtomMediaType.Formatter);
                if (feed.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance))
                {
                    return new ResolvingEncounter(currentResponse, applicationStateInfo);
                }
                return new Error(currentResponse, applicationStateInfo);
            }

            var entry = currentResponse.Content.ReadAsObject<SyndicationItem>(AtomMediaType.Formatter);

            if (entry.Title.Text.Equals("Exit"))
            {
                return new GoalAchieved(currentResponse);
            }

            var exitLink = GetExitLink(entry, applicationStateInfo.History, "north", "east", "west", "south");

            var newResponse = client.Get(new Uri(entry.BaseUri, exitLink.Uri));
            var exitUri = applicationStateInfo.History.Contains(exitLink.Uri) ? null : exitLink.Uri;
            
            return new Exploring(newResponse, applicationStateInfo.GetBuilder().AddToHistory(exitUri).Build());
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

        public ApplicationStateInfo ApplicationStateInfo
        {
            get { return applicationStateInfo; }
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}