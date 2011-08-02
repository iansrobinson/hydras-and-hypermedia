using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Server.Resources
{
    [ServiceContract]
    public class News
    {
        [WebGet(UriTemplate="{id}")]
        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
        {
            var feed = new SyndicationFeed();
            var response = new HttpResponseMessage<SyndicationFeed>(feed, HttpStatusCode.OK);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(AtomFormatter.Value);
            return response;
        }
    }
}