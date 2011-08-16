using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using RestInPractice.Server.Domain;

namespace RestInPractice.Server.Resources
{
    public class EncounterResource
    {
        public EncounterResource(Repository<Encounter> encounters)
        {
        }

        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage<SyndicationItem> Post(string id, HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}