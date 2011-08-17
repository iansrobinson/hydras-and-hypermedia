using System;
using System.Net.Http;
using System.ServiceModel.Syndication;
using HydrasAndHypermedia.Server.Domain;
using Microsoft.ApplicationServer.Http;

namespace HydrasAndHypermedia.Server.Resources
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