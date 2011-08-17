using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using HydrasAndHypermedia.Client.Comparers;
using HydrasAndHypermedia.Client.Extensions;
using HydrasAndHypermedia.Client.Xhtml;
using HydrasAndHypermedia.MediaTypes;

namespace HydrasAndHypermedia.Client.ApplicationStates
{
    public class ResolvingEncounter : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;
        private readonly ApplicationStateInfo applicationStateInfo;

        public ResolvingEncounter(HttpResponseMessage currentResponse, ApplicationStateInfo applicationStateInfo)
        {
            this.currentResponse = currentResponse;
            this.applicationStateInfo = applicationStateInfo;
        }

        public IApplicationState NextState(HttpClient client)
        {
            if (applicationStateInfo.Endurance < 1)
            {
                return new Defeated(currentResponse, applicationStateInfo);
            }

            if (currentResponse.Content.Headers.ContentType.Equals(AtomMediaType.Feed))
            {
                var feed = currentResponse.Content.ReadAsObject<SyndicationFeed>(AtomMediaType.Formatter);
                if (feed.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance))
                {
                    var form = Form.ParseFromFeedExtension(feed);
                    form.Fields.Named("endurance").Value = applicationStateInfo.Endurance.ToString();

                    var newResponse = client.Send(form.CreateRequest(feed.BaseUri));

                    if (newResponse.Content.Headers.ContentType.Equals(AtomMediaType.Entry))
                    {
                        var newContent = newResponse.Content.ReadAsObject<SyndicationItem>(AtomMediaType.Formatter);
                        var newForm = Form.ParseFromEntryContent(newContent);
                        var newEndurance = int.Parse(newForm.Fields.Named("endurance").Value);

                        return new ResolvingEncounter(newResponse, applicationStateInfo.GetBuilder().UpdateEndurance(newEndurance).Build());
                    }
                }

                return new Error(currentResponse, applicationStateInfo);
            }

            if (currentResponse.Content.Headers.ContentType.Equals(AtomMediaType.Entry))
            {
                var entry = currentResponse.Content.ReadAsObject<SyndicationItem>(AtomMediaType.Formatter);

                if (entry.Categories.Contains(new SyndicationCategory("room"), CategoryComparer.Instance))
                {
                    return new Exploring(currentResponse, applicationStateInfo);
                }

                var continueLink = entry.Links.FirstOrDefault(l => l.RelationshipType.Equals("continue"));
                if (continueLink != null)
                {
                    return new ResolvingEncounter(client.Get(new Uri(entry.BaseUri, continueLink.Uri)), applicationStateInfo);
                }

                var form = Form.ParseFromEntryContent(entry);
                var newResponse = client.Send(form.CreateRequest(entry.BaseUri));

                if (newResponse.Content.Headers.ContentType.Equals(AtomMediaType.Entry))
                {
                    var newContent = newResponse.Content.ReadAsObject<SyndicationItem>(AtomMediaType.Formatter);
                    var newForm = Form.ParseFromEntryContent(newContent);
                    var newEndurance = int.Parse(newForm.Fields.Named("endurance").Value);

                    return new ResolvingEncounter(newResponse, applicationStateInfo.GetBuilder().UpdateEndurance(newEndurance).Build());
                }
            }

            return new Error(currentResponse, applicationStateInfo);
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