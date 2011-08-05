using System;
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
            body.Authors.Add(new SyndicationPerson { Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com" });

            var response = new HttpResponseMessage<SyndicationFeed>(body){StatusCode = HttpStatusCode.OK };
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};           
            response.Content.Headers.ContentType = AtomMediaType.Feed;

            return response;
        }
    }
}