using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Xhtml;

namespace RestInPractice.Server.Resources
{
    public class EncounterResource
    {
        private readonly Repository<Encounter> encounters;

        public EncounterResource(Repository<Encounter> encounters)
        {
            this.encounters = encounters;
        }

        public HttpResponseMessage<SyndicationFeed> Get(string id, HttpRequestMessage request)
        {
            Encounter encounter;
            try
            {
                encounter = encounters.Get(int.Parse(id));
            }
            catch (KeyNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            

            var feed = new SyndicationFeed
                           {
                               Id = "tag:restinpractice.com,2011-09-05:/encounters/" + encounter.Id,
                               BaseUri = new Uri("http://localhost:8081"),
                               Title = SyndicationContent.CreatePlaintextContent(encounter.Title),
                               Description = SyndicationContent.CreatePlaintextContent(encounter.Description)
                           };
            feed.Categories.Add(new SyndicationCategory("encounter"));
            feed.Authors.Add(new SyndicationPerson {Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com"});
            feed.Links.Add(new SyndicationLink {RelationshipType = "flee", Uri = new Uri("/rooms/" + encounter.FleeRoomId, UriKind.Relative)});

            var xhtml = new FormWriter(new Uri("/encounters/" + encounter.Id, UriKind.RelativeOrAbsolute),HttpMethod.Post, new TextInput("endurance")).ToXhtml();
            feed.ElementExtensions.Add(XmlReader.Create(new StringReader(xhtml)));

            feed.Items = encounter.GetAllRounds()
                .Reverse()
                .Select(o =>
                            {
                                var entry = new SyndicationItem
                                                          {
                                                              Title = SyndicationContent.CreatePlaintextContent("Round " + o.Id),
                                                              Summary = SyndicationContent.CreatePlaintextContent(string.Format("The {0} has {1} Endurance Points", encounter.Title, o.Endurance))
                                                          };
                                entry.Categories.Add(new SyndicationCategory("round"));
                                return entry;
                            });


            var response = new HttpResponseMessage<SyndicationFeed>(feed) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
            response.Content.Headers.ContentType = AtomMediaType.Feed;

            return response;
        }

        public HttpResponseMessage<SyndicationItem> Post(string id, HttpRequestMessage<ObjectContent<FormUrlEncodedContent>> request)
        {
            return new HttpResponseMessage<SyndicationItem>(HttpStatusCode.Created);
        }
    }
}