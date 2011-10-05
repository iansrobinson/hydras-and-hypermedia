using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using HydrasAndHypermedia.MediaTypes;
using HydrasAndHypermedia.Server.Domain;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace HydrasAndHypermedia.Server.Resources
{
    [ServiceContract]
    public class RoomResource
    {
        private readonly Repository<Room> rooms;
        private readonly Repository<Encounter> encounters;

        public RoomResource(Repository<Room> rooms, Repository<Encounter> encounters)
        {
            this.rooms = rooms;
            this.encounters = encounters;
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

            if (room.IsGuarded(encounters))
            {
                var seeOtherResponse = new HttpResponseMessage {StatusCode = HttpStatusCode.SeeOther};
                seeOtherResponse.Headers.Location = new Uri(string.Format("http://" + Environment.MachineName + ":8081/encounters/{0}", room.GetEncounter(encounters).Id));
                throw new HttpResponseException(seeOtherResponse);
            }

            var entry = new SyndicationItem
                            {
                                BaseUri = new Uri("http://" + Environment.MachineName + ":8081"),
                                Title = SyndicationContent.CreatePlaintextContent(room.Title),
                                Summary = SyndicationContent.CreatePlaintextContent(room.Description)
                            };

            entry.Authors.Add(new SyndicationPerson {Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com"});
            entry.Categories.Add(new SyndicationCategory("room"));

            foreach (var exit in room.Exits)
            {
                var link = new SyndicationLink
                               {
                                   Uri = new Uri("/rooms/" + exit.RoomId, UriKind.Relative),
                                   RelationshipType = exit.Direction.ToString().ToLower()
                               };
                entry.Links.Add(link);
            }

            var response = new HttpResponseMessage<SyndicationItem>(entry) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(0, 0, 0, 10)};
            response.Content.Headers.ContentType = AtomMediaType.Entry;

            return response;
        }
    }
}