using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Xhtml;

namespace RestInPractice.Server.Resources
{
    [ServiceContract]
    public class EncounterResource
    {
        private static readonly Uri BaseUri = new Uri(string.Format("http://{0}:8081/", Environment.MachineName));

        private readonly Repository<Encounter> encounters;

        public EncounterResource(Repository<Encounter> encounters)
        {
            this.encounters = encounters;
        }

        [WebGet(UriTemplate = "{id}")]
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
                               BaseUri = BaseUri,
                               Title = SyndicationContent.CreatePlaintextContent(encounter.Title),
                               Description = SyndicationContent.CreatePlaintextContent(encounter.Description)
                           };
            feed.Categories.Add(new SyndicationCategory("encounter"));
            feed.Authors.Add(new SyndicationPerson {Name = "Dungeon Master", Email = "dungeon.master@restinpractice.com"});

            if (encounter.IsResolved)
            {
                feed.Links.Add(new SyndicationLink {RelationshipType = "continue", Uri = new Uri("/rooms/" + encounter.GuardedRoomId, UriKind.Relative)});
            }
            else
            {
                feed.Links.Add(new SyndicationLink {RelationshipType = "flee", Uri = new Uri("/rooms/" + encounter.FleeRoomId, UriKind.Relative)});
                var xhtml = new FormWriter(new Uri("/encounters/" + encounter.Id, UriKind.RelativeOrAbsolute), HttpMethod.Post, new TextInput("endurance")).ToXhtml();
                feed.ElementExtensions.Add(XmlReader.Create(new StringReader(xhtml)));
            }

            feed.Items = encounter.GetAllRounds()
                .Reverse()
                .Select(round =>
                            {
                                var entry = new SyndicationItem
                                                {
                                                    Id = string.Format("tag:restinpractice.com,2011-09-05:/encounters/{0}/round/{1}", encounter.Id, round.Id),
                                                    Title = SyndicationContent.CreatePlaintextContent("Round " + round.Id),
                                                    Summary = SyndicationContent.CreatePlaintextContent(string.Format("The {0} has {1} Endurance Points", encounter.Title, round.Endurance))
                                                };
                                entry.Links.Add(SyndicationLink.CreateSelfLink(new Uri(string.Format("http://{0}:8081/encounters/{1}/round/{2}", Environment.MachineName, encounter.Id, round.Id))));
                                entry.Categories.Add(new SyndicationCategory("round"));
                                return entry;
                            });


            var response = new HttpResponseMessage<SyndicationFeed>(feed) {StatusCode = HttpStatusCode.OK};
            response.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true, NoStore = true};
            response.Content.Headers.ContentType = AtomMediaType.Feed;
            response.Content.Headers.ContentType.CharSet = "UTF-8";

            return response;
        }

        [WebInvoke(Method="POST", UriTemplate = "{id}")]
        public HttpResponseMessage<SyndicationItem> Post(string id, HttpRequestMessage request)
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

            if (encounter.IsResolved)
            {
                var methodNotAllowedResponse = new HttpResponseMessage {StatusCode = HttpStatusCode.MethodNotAllowed, Content = new ByteArrayContent(new byte[] {})};
                methodNotAllowedResponse.Content.Headers.Allow.Add("GET");
                throw new HttpResponseException(methodNotAllowedResponse);
            }

            var form = request.Content.ReadAs<JsonValue>(new[] {new FormUrlEncodedMediaTypeFormatter()});
            var clientEndurance = form["endurance"].ReadAs<int>();

            var result = encounter.Action(clientEndurance);
            var round = result.Round;

            var xhtml = new FormWriter(new Uri("/encounters/" + encounter.Id, UriKind.RelativeOrAbsolute), HttpMethod.Post, new TextInput("endurance", result.ClientEndurance.ToString())).ToXhtml();

            var entry = new SyndicationItem
                            {
                                Id = string.Format("tag:restinpractice.com,2011-09-05:/encounters/{0}/round/{1}", encounter.Id, round.Id),
                                BaseUri = BaseUri,
                                Title = SyndicationContent.CreatePlaintextContent("Round " + round.Id),
                                Summary = SyndicationContent.CreatePlaintextContent(string.Format("The {0} has {1} Endurance Point{2}", encounter.Title, round.Endurance, Math.Abs(round.Endurance).Equals(1) ? "" : "s")),
                                Content = SyndicationContent.CreateXhtmlContent(xhtml),
                            };
            entry.Links.Add(SyndicationLink.CreateSelfLink(new Uri(string.Format("http://{0}:8081/encounters/{1}/round/{2}", Environment.MachineName, encounter.Id, round.Id))));
            entry.Categories.Add(new SyndicationCategory("round"));

            if (encounter.IsResolved)
            {
                entry.Links.Add(new SyndicationLink {RelationshipType = "continue", Uri = new Uri("/rooms/" + encounter.GuardedRoomId, UriKind.Relative)});
            }
            else
            {
                entry.Content = SyndicationContent.CreateXhtmlContent(xhtml);
                entry.Links.Add(new SyndicationLink {RelationshipType = "flee", Uri = new Uri("/rooms/" + encounter.FleeRoomId, UriKind.Relative)});
            }

            var response = new HttpResponseMessage<SyndicationItem>(entry) {StatusCode = HttpStatusCode.Created};
            response.Headers.Location = new Uri(string.Format("http://{0}:8081/encounters/{1}/round/{2}", Environment.MachineName, encounter.Id, result.Round.Id));
            response.Content.Headers.ContentType = AtomMediaType.Entry;
            response.Content.Headers.ContentType.CharSet = "UTF-8";
           
            return response;
        }
    }
}