using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using RestInPractice.Server.Resources;

namespace RestInPractice.Exercises.Exercise01
{
    [TestFixture]
    public class RoomResourceTests
    {
        [Test]
        public void Returns200Ok()
        {
            var resource = new RoomResource();
            var response = resource.Get("1", new HttpRequestMessage());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void ResponseIsPublicallyCacheable()
        {
            var resource = new RoomResource();
            var response = resource.Get("1", new HttpRequestMessage());
            Assert.IsTrue(response.Headers.CacheControl.Public);
        }

        [Test]
        public void ResponseCanBeCachedFor10Seconds()
        {
            var resource = new RoomResource();
            var response = resource.Get("1", new HttpRequestMessage());
            Assert.AreEqual(new TimeSpan(0, 0, 0, 10), response.Headers.CacheControl.MaxAge);
        }

        [Test]
        public void ResponseContentTypeIsApplicationAtomPlusXml()
        {
            var resource = new RoomResource();
            var response = resource.Get("1", new HttpRequestMessage());
            Assert.AreEqual("application/atom+xml", response.Content.Headers.ContentType.MediaType);
        }
        
    }
}