﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestInPractice.Server.Domain;

namespace RestInPractice.Server.Resources
{
    public class RoomResource
    {
        private readonly Rooms rooms;

        public RoomResource(Rooms rooms)
        {
            this.rooms = rooms;
        }

        [WebGet]
        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
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

            var body = new SyndicationFeed
                           {
                               BaseUri = new Uri("http://localhost/"),
                               Title = SyndicationContent.CreatePlaintextContent(room.Title),
                               Description = SyndicationContent.CreatePlaintextContent(room.Description)
                           };

            foreach (var exit in room.Exits)
            {
                var link = new SyndicationLink
                               {
                                   Uri = new Uri("/rooms/" + exit.RoomId, UriKind.Relative),
                                   RelationshipType = exit.Direction.ToString().ToLower()
                               };
                body.Links.Add(link);
            }

            var response = new HttpResponseMessage<SyndicationFeed>(body) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {Public = true, MaxAge = new TimeSpan(0, 0, 0, 10)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");

            return response;
        }
    }
}