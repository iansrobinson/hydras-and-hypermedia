using System.Net.Http;
using System.Text;
using NUnit.Framework;
using RestInPractice.Client.ApplicationStates;
using RestInPractice.Exercises.Helpers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Exercise03
{
    [TestFixture]
    public class Part03_ClientTests
    {
        [Test]
        public void ShouldReturnResolvingEncounterApplicationStateIfCurrentResponseContainsEncounterFeed()
        {
            var feed = new FeedBuilder().WithCategory("encounter").ToString();

            var currentResponse = new HttpResponseMessage {Content = new StringContent(feed, Encoding.Unicode)};
            currentResponse.Content.Headers.ContentType = AtomMediaType.FeedValue;

            var initialState = new Exploring(currentResponse);
            var nextState = initialState.NextState(new HttpClient());

            Assert.IsInstanceOf(typeof(ResolvingEncounter), nextState);
            Assert.AreEqual(currentResponse, nextState.CurrentResponse);
        }
    }
}