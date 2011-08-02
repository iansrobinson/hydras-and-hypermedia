using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using NUnit.Framework;
using RestInPractice.Client;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise02
{
    [TestFixture]
    public class Part03_FunctionalTests
    {
        [Test]
        public void ShouldNavigateMaze()
        {
            var configuration = HttpHostConfiguration.Create()
                .SetResourceFactory((type, instanceContext, request) => new RoomResource(Maze.Instance), (instanceContext, obj) => { })
                .AddFormatters(AtomMediaType.Formatter);

            using (var host = new HttpConfigurableServiceHost(typeof (RoomResource), configuration, new Uri("http://localhost:8081/rooms/")))
            {
                host.Open();

                var moveCount = 0;

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value));

                IApplicationState state = new Started(new Uri("http://localhost:8081/rooms/1"));
                while (!state.IsTerminalState && moveCount++ < 20)
                {
                    state = state.NextState(httpClient);
                    if (state.GetType().Equals(typeof (Exploring)))
                    {
                        Console.WriteLine(state.CurrentResponse.RequestMessage.RequestUri.AbsoluteUri);
                    }
                }

                Assert.IsInstanceOf(typeof (GoalAchieved), state);

                host.Close();
            }
        }
    }
}