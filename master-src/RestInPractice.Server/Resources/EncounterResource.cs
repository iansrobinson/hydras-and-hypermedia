using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using Microsoft.ApplicationServer.Http;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;

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
            var encounter = encounters.Get(int.Parse(id));

            var body = new SyndicationFeed
                           {
                               Id = "tag:restinpractice.com,2011-09-05:/encounters/" + encounter.Id,
                               BaseUri = new Uri("http://localhost:8081"),
                               Title = SyndicationContent.CreatePlaintextContent(encounter.Title),
                               Description = SyndicationContent.CreatePlaintextContent(encounter.Description)
                           };
            body.Categories.Add(new SyndicationCategory("encounter"));
            body.Authors.Add(new SyndicationPerson {Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com"});
            body.Links.Add(new SyndicationLink {RelationshipType = "flee", Uri = new Uri("/rooms/" + encounter.FleeRoomId, UriKind.Relative)});


            body.Items = encounter.GetAllOutcomes()
                .Reverse()
                .Select(o =>
                            {
                                var entry = new SyndicationItem
                                                          {
                                                              Title = SyndicationContent.CreatePlaintextContent("Round " + o.Id),
                                                              Summary = SyndicationContent.CreatePlaintextContent(string.Format("The {0} has {1} Endurance Points", encounter.Title, o.Endurance))
                                                          };
                                entry.Categories.Add(new SyndicationCategory("outcome"));
                                return entry;
                            });


            var response = new HttpResponseMessage<SyndicationFeed>(body) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
            response.Content.Headers.ContentType = AtomMediaType.Feed;

            return response;
        }
    }
}