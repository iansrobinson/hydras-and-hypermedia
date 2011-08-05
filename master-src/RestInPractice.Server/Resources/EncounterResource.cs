using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using RestInPractice.MediaTypes;

namespace RestInPractice.Server.Resources
{
    public class EncounterResource
    {
        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
        {
            var body = new SyndicationFeed();

            var response = new HttpResponseMessage<SyndicationFeed>(body) { StatusCode = HttpStatusCode.OK };
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};           
            response.Content.Headers.ContentType = AtomMediaType.Feed;

            return response;
        }
    }
}