using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using HydrasAndHypermedia.Exercises.Helpers;
using HydrasAndHypermedia.MediaTypes;
using HydrasAndHypermedia.Server.Resources;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;

namespace HydrasAndHypermedia.Exercises.Exercise01
{
    [TestFixture]
    public class Part03_FunctionalTests
    {
        //Before running this test for the first time, you must reserve the http://+:8081 namespace
        //Open a command propmpt as Administrator, and type:
        //netsh http add urlacl url=http://+:8081/ user="<Machine Name>\<User Name>"
        [Test]
        public void FunctionalTest()
        {
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Maze.NewInstance(), Monsters.NullEncounters()), (instanceContext, obj) => { });

            // Workaround for serialization issue in Preview 4. 
            // Must clear default XML formatter from Formatters before adding Atom formatter.
            var hostConfiguration = (HttpHostConfiguration) configuration;
            hostConfiguration.OperationHandlerFactory.Formatters.Clear();
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(0, AtomMediaType.Formatter);

            using (var host = new HttpConfigurableServiceHost(typeof (RoomResource), configuration, new Uri("http://" + Environment.MachineName + ":8081/rooms/")))
            {
                host.Open();

                var entryFormatter = new Atom10ItemFormatter();
                var client = AtomClient.CreateDefault();

                using (var firstResponse = client.Get("http://" + Environment.MachineName + ":8081/rooms/1"))
                {
                    entryFormatter.ReadFrom(XmlReader.Create(firstResponse.Content.ContentReadStream));
                }

                var firstRoom = entryFormatter.Item;
                var northLink = firstRoom.Links.First(l => l.RelationshipType.Equals("north"));
                var northUri = new Uri(firstRoom.BaseUri, northLink.Uri);

                using (var secondResponse = client.Get(northUri))
                {
                    entryFormatter.ReadFrom(XmlReader.Create(secondResponse.Content.ContentReadStream));
                }

                var nextRoom = entryFormatter.Item;

                //See Maze class for layout of the maze. Room 4 is north of room 1.
                Assert.AreEqual(Maze.NewInstance().Get(4).Description, nextRoom.Summary.Text);

                host.Close();
            }
        }
    }
}