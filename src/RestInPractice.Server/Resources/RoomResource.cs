using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using RestInPractice.Server.Domain;

namespace RestInPractice.Server.Resources
{
    public class RoomResource
    {
        public RoomResource(Repository<Room> rooms, Repository<Encounter> encounters)
        {
        }

        public HttpResponseMessage<SyndicationItem> Get(string id, HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}