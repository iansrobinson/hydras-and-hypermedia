﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Formatters;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class Part03_FunctionalTests
    {
        [Test]
        public void FunctionalTest()
        {
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Rooms.Instance), (instanceContext, obj) => { })
                .AddFormatters(AtomMediaType.Instance);

            using (var host = new HttpConfigurableServiceHost(typeof (RoomResource), configuration, new Uri("http://localhost:8081/rooms/")))
            {
                host.Open();

                var entryFormatter = new Atom10ItemFormatter();
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value));

                using (var firstResponse = client.Get("http://localhost:8081/rooms/1"))
                {
                    entryFormatter.ReadFrom(XmlReader.Create(firstResponse.Content.ContentReadStream));
                }

                var room1 = entryFormatter.Item;
                var northLink = room1.Links.First(l => l.RelationshipType.Equals("north"));
                var northUri = new Uri(room1.BaseUri, northLink.Uri);

                using (var secondResponse = client.Get(northUri))
                {
                    entryFormatter.ReadFrom(XmlReader.Create(secondResponse.Content.ContentReadStream));
                }

                var room2 = entryFormatter.Item;

                Assert.AreEqual(Rooms.Instance.Get(2).Description, room2.Summary.Text);

                host.Close();
            }
        }
    }
}