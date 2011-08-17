using System;
using System.Collections.Generic;
using System.Linq;
using HydrasAndHypermedia.Client;
using HydrasAndHypermedia.Client.ApplicationStates;
using HydrasAndHypermedia.Exercises.Helpers;
using HydrasAndHypermedia.MediaTypes;
using HydrasAndHypermedia.Server.Resources;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;

namespace HydrasAndHypermedia.Exercises.Exercise03
{
    [TestFixture]
    public class Part08_FunctionalTests
    {
        [Test]
        public void ShouldDefeatMonsterAndNavigateMaze()
        {
            var expectedPath = new[]
                                   {
                                       CreateRoomPath(1),
                                       CreateEncounterPath(1),
                                       CreateRoomPath(4),
                                       CreateRoomPath(7),
                                       CreateRoomPath(6),
                                       CreateRoomPath(5),
                                       CreateRoomPath(4),
                                       CreateRoomPath(8),
                                       CreateRoomPath(9),
                                       CreateRoomPath(8),
                                       CreateRoomPath(10)
                                   };

            var path = new List<string>();
            var encounterRepository = Monsters.NewInstance();

            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) =>
                                        {
                                            if (type.Equals(typeof (RoomResource)))
                                            {
                                                return new RoomResource(Maze.NewInstance(), encounterRepository);
                                            }
                                            if (type.Equals(typeof (EncounterResource)))
                                            {
                                                return new EncounterResource(encounterRepository);
                                            }
                                            throw new ArgumentException("Unrecognized type: " + type.FullName, "type");
                                        }, (instanceContext, obj) => { });

            // Workaround for serialization issue in Preview 4. 
            // Must clear default XML formatter from Formatters before adding Atom formatter.
            var hostConfiguration = (HttpHostConfiguration) configuration;
            hostConfiguration.OperationHandlerFactory.Formatters.Clear();
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(0, AtomMediaType.Formatter);
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(1, new FormUrlEncodedMediaTypeFormatter());

            using (HttpConfigurableServiceHost roomHost = new HttpConfigurableServiceHost(typeof (RoomResource), configuration, new Uri("http://" + Environment.MachineName + ":8081/rooms/")),
                                               encounterHost = new HttpConfigurableServiceHost(typeof (EncounterResource), configuration, new Uri("http://" + Environment.MachineName + ":8081/encounters/")))
            {
                roomHost.Open();
                encounterHost.Open();

                var moveCount = 0;
                var client = AtomClient.CreateDefault();

                IApplicationState state = new Started(new Uri("http://" + Environment.MachineName + ":8081/rooms/1"), ApplicationStateInfo.WithEndurance(5));
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

                encounterHost.Close();
                roomHost.Close();
            }
        }

        private static string CreateRoomPath(int roomId)
        {
            return string.Format("http://{0}:8081/rooms/{1}", Environment.MachineName.ToLower(), roomId);
        }

        private static string CreateEncounterPath(int roomId)
        {
            return string.Format("http://{0}:8081/encounters/{1}", Environment.MachineName.ToLower(), roomId);
        }
    }
}