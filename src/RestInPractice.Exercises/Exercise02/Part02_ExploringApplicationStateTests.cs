using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using NUnit.Framework;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise02
{
    [TestFixture]
    public class Part02_ExploringApplicationStateTests
    {
        private static readonly Uri NorthUri = new Uri("http://localhost/rooms/10");
        private static readonly Uri SouthUri = new Uri("http://localhost/rooms/11");
        private static readonly Uri EastUri = new Uri("http://localhost/rooms/12");
        private static readonly Uri WestUri = new Uri("http://localhost/rooms/13");

        [Test]
        public void ShouldFollowExitToNorthInPreferenceToAllOtherExits()
        {
            var entry = new EntryBuilder()
                .WithNorthLink(NorthUri)
                .WithSouthLink(SouthUri)
                .WithEastLink(EastUri)
                .WithWestLink(WestUri)
                .ToString();

            var currentResponse = new HttpResponseMessage {Content = new StringContent(entry, Encoding.Unicode)};
            currentResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(AtomMediaType.Value);

            var newResponse = new HttpResponseMessage();
            var mockEndpoint = new StubEndpoint(request => request.RequestUri.Equals(NorthUri) ? newResponse : null);

            var client = new HttpClient {Channel = mockEndpoint};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value));

            var state = new Exploring(currentResponse);
            var nextState = state.NextState(client);

            Assert.AreEqual(newResponse, nextState.CurrentResponse);
        }
    }
}