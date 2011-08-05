using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;
using RestInPractice.Client;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Domain;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise02
{
    [TestFixture]
    public class Part03_FunctionalTests
    {
        [Test]
        public void ShouldNavigateMaze()
        {
            var expectedPath = new[]
                                   {
                                       "http://localhost:8081/rooms/1",
                                       "http://localhost:8081/rooms/4",
                                       "http://localhost:8081/rooms/7",
                                       "http://localhost:8081/rooms/6",
                                       "http://localhost:8081/rooms/5",
                                       "http://localhost:8081/rooms/4",
                                       "http://localhost:8081/rooms/8",
                                       "http://localhost:8081/rooms/9",
                                       "http://localhost:8081/rooms/8",
                                       "http://localhost:8081/rooms/10"
                                   };

            var path = new List<string>();

            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Maze.Instance, new Repository<Encounter>()), (instanceContext, obj) => { });

            // Workaround for serialization issue in Preview 4. 
            // Must clear default XML formatter from Formatters before adding Atom formatter.
            var hostConfiguration = (HttpHostConfiguration) configuration;
            hostConfiguration.OperationHandlerFactory.Formatters.Clear();
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(0, AtomMediaType.Formatter);

            using (var host = new HttpConfigurableServiceHost(typeof (RoomResource), configuration, new Uri("http://localhost:8081/rooms/")))
            {
                host.Open();

                var moveCount = 0;
                var client = AtomClient.CreateDefault();

                IApplicationState state = new Started(new Uri("http://localhost:8081/rooms/1"), ApplicationStateInfo.WithEndurance(5));
                while (!state.IsTerminalState && moveCount++ < 20)
                {
                    state = state.NextState(client);
                    if (state.GetType().Equals(typeof (Exploring)))
                    {
                        path.Add(state.CurrentResponse.RequestMessage.RequestUri.AbsoluteUri);
                    }
                }

                Assert.IsInstanceOf(typeof (GoalAchieved), state);
                Assert.IsTrue(path.SequenceEqual(expectedPath));

                host.Close();
            }
        }
    }
}