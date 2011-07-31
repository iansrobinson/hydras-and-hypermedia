using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var body = new SyndicationFeed();
            
            var response = new HttpResponseMessage<SyndicationFeed>(body){StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(0, 0, 0, 10)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            
            return response;
        }
    }
}