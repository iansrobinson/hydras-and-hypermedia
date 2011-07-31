using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Server.Resources
{
    public class RoomResource
    {
        [WebGet]
        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
        {
            return new HttpResponseMessage<SyndicationFeed>(HttpStatusCode.OK);
        }
    }
}