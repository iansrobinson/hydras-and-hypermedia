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
    }
}