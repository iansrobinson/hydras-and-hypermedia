using System;
using System.Collections.Generic;
using System.Linq;
using HydrasAndHypermedia.Client;
using HydrasAndHypermedia.Client.ApplicationStates;
using HydrasAndHypermedia.Exercises.Helpers;
using HydrasAndHypermedia.MediaTypes;
using HydrasAndHypermedia.Server.Resources;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;

namespace HydrasAndHypermedia.Exercises.Exercise02
{
    [TestFixture]
    public class Part02_FunctionalTests
    {
        [Test]
        public void ShouldNavigateMaze()
        {
            var expectedPath = new[]
                                   {
                                       CreatePath(1),
                                       CreatePath(4),
                                       CreatePath(7),
                                       CreatePath(6),
                                       CreatePath(5),
                                       CreatePath(4),
                                       CreatePath(8),
                                       CreatePath(9),
                                       CreatePath(8),
                                       CreatePath(10)
                                   };

            var path = new List<string>();

            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Maze.NewInstance(), Monsters.NullEncounters()), (instanceContext, obj) => { });

            // Workaround for serialization issue in Preview 4. 
            // Must clear default XML formatter from Formatters before adding Atom formatter.
            var hostConfiguration = (HttpHostConfiguration) configuration;
            hostConfiguration.OperationHandlerFactory.Formatters.Clear();
            hostConfiguration.OperationHandlerFactory.Formatters.Insert(0, AtomMediaType.Formatter);

            using (var host = new HttpConfigurableServiceHost(typeof(RoomResource), configuration, new Uri("http://" + Environment.MachineName + ":8081/rooms/")))
            {
                host.Open();

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

                host.Close();
            }
        }

        private static string CreatePath(int roomId)
        {
            return string.Format("http://{0}:8081/rooms/{1}", Environment.MachineName.ToLower(), roomId);
        }
    }
}