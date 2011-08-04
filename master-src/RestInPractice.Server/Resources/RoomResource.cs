using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;

namespace RestInPractice.Server.Resources
{
    [ServiceContract]
    public class RoomResource
    {
        private readonly Repository<Room> rooms;

        public RoomResource(Repository<Room> rooms, Repository<Encounter> encounters)
        {
            this.rooms = rooms;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<SyndicationItem> Get(string id, HttpRequestMessage request)
        {
            Room room;
            try
            {
                room = rooms.Get(int.Parse(id));
            }
            catch (KeyNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var body = new SyndicationItem
                           {
                               Id = "tag:restinpractice.com,2011-09-05:/rooms/" + room.Id,
                               BaseUri = new Uri("http://localhost:8081"),
                               Title = SyndicationContent.CreatePlaintextContent(room.Title),
                               Summary = SyndicationContent.CreatePlaintextContent(room.Description)
                           };

            body.Authors.Add(new SyndicationPerson{Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com"});

            foreach (var exit in room.Exits)
            {
                var link = new SyndicationLink
                               {
                                   Uri = new Uri("/rooms/" + exit.RoomId, UriKind.Relative),
                                   RelationshipType = exit.Direction.ToString().ToLower()
                               };
                body.Links.Add(link);
            }

            var response = new HttpResponseMessage<SyndicationItem>(body) { StatusCode = HttpStatusCode.OK };
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(0, 0, 0, 10)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(AtomMediaType.Value);

            return response;
        }
    }
}