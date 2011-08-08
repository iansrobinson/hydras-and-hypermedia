using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;
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
            var encounter = encounters.Get(int.Parse(id));

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

            var xhtml = new FormWriter(new Uri("/encounters/1", UriKind.RelativeOrAbsolute),HttpMethod.Post, new TextInput("name")).ToXhtml();
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
                                entry.Content = SyndicationContent.CreateXhtmlContent(@"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""post"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""endurance""/>
  </form>
</div>");
                                return entry;
                            });


            var response = new HttpResponseMessage<SyndicationFeed>(feed) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
            response.Content.Headers.ContentType = AtomMediaType.Feed;

            return response;
        }
    }
}