using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using RestInPractice.Client.Comparers;
using RestInPractice.Client.Extensions;
using RestInPractice.Client.Xhtml;
using RestInPractice.MediaTypes;

namespace RestInPractice.Client.ApplicationStates
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
            if (currentResponse.Content.Headers.ContentType.Equals(AtomMediaType.Feed))
            {
                var feed = currentResponse.Content.ReadAsObject<SyndicationFeed>(AtomMediaType.Formatter);
                if (feed.Categories.Contains(new SyndicationCategory("room"), CategoryComparer.Instance))
                {
                    return new Exploring(currentResponse, applicationStateInfo);
                }
                if (feed.Categories.Contains(new SyndicationCategory("encounter"), CategoryComparer.Instance))
                {
                    var form = Form.ParseFromFeedExtension(feed);
                    form.Fields.Named("endurance").Value = applicationStateInfo.Endurance.ToString();

                    var newResponse = client.Send(form.CreateRequest(feed.BaseUri));
                    var newContent = newResponse.Content.ReadAsObject<SyndicationItem>(AtomMediaType.Formatter);
                    var newForm = Form.ParseFromEntryContent(newContent);
                    var newEndurance = int.Parse(newForm.Fields.Named("endurance").Value);

                    return new ResolvingEncounter(newResponse, applicationStateInfo.GetBuilder().UpdateEndurance(newEndurance).Build());
                }
                return new Error(currentResponse, applicationStateInfo);
            }

            return null;
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