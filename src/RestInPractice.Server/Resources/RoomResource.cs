using System.Net.Http;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using RestInPractice.Server.Domain;

namespace RestInPractice.Server.Resources
{
    public class RoomResource
    {
        public RoomResource(Rooms rooms)
        {
        }

        public HttpResponseMessage<SyndicationItemFormatter> Get(string id, HttpRequestMessage request)
        {
            return null;
        }
    }
}