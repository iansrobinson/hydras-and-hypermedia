using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using HydrasAndHypermedia.Server.Domain;
using Microsoft.ApplicationServer.Http;

namespace HydrasAndHypermedia.Server.Resources
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