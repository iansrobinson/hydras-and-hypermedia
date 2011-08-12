using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                    var form = FormReader.Read(feed);
                    form.TextInputFields.First().Value = applicationStateInfo.Endurance.ToString();
                    var request = new HttpRequestMessage {Method = form.Method, RequestUri = form.Action, Content = form.ToFormUrlEncodedContent()};
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(form.Enctype);
                    client.Send(request);
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