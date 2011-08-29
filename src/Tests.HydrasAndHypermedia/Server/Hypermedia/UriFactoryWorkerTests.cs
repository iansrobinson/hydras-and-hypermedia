using System;
using NUnit.Framework;
using HydrasAndHypermedia.Server.Hypermedia;

namespace Tests.HydrasAndHypermedia.Server.Hypermedia
{
    [TestFixture]
    public class UriFactoryWorkerTests
    {
        [Test]
        public void ShouldGenerateRelativeUriFromRoutePrefixAndTemplateAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("encounters", "{encounterId}/rounds/{roundId}");
            Assert.AreEqual("encounters/1/rounds/2", uriFactory.CreateRelativeUri("1", "2").ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriFromBaseAddressAndRoutePrefixAndTempleAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("encounters", "{encounterId}/rounds/{roundId}");
            Assert.AreEqual("http://restinpractice.com/encounters/1/rounds/2", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com"), "1", "2").ToString());
        }

        [Test]
        public void ShouldUseAllOfTheSuppliedBaseAddressIfTerminatedWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "{encounterId}/rounds/{roundId}");
            Assert.AreEqual("http://restinpractice.com:8080/virtual-directory/encounters/1/rounds/2", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com:8080/virtual-directory/"), "1", "2").ToString());
        }

        [Test]
        public void ShouldUseTheSuppliedBaseAddressUpToLastBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "{encounterId}/rounds/{roundId}");
            Assert.AreEqual("http://restinpractice.com:8080/virtual-directory/encounters/1/rounds/2", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com:8080/virtual-directory/suffix"), "1", "2").ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("encounters");
            Assert.AreEqual("encounters", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithTerminatingBackslashWhenTemplateEndsWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "current/");
            Assert.AreEqual("encounters/current/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "/");
            Assert.AreEqual("encounters/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("encounters");
            Assert.AreEqual("http://restinpractice.com/encounters", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com")).ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithTerminatingBackslashWhenTemplateEndsWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "current/");
            Assert.AreEqual("http://restinpractice.com/encounters/current/", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com")).ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("encounters", "/");
            Assert.AreEqual("http://restinpractice.com/encounters/", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com")).ToString());
        }

        [Test]
        public void ShouldKeepStartingBackslashOnUriTemplateValue()
        {
            var uriFactory = new UriFactoryWorker("rooms", "/?a=b");
            Assert.AreEqual("http://restinpractice.com/rooms/?a=b", uriFactory.CreateAbsoluteUri(new Uri("http://restinpractice.com")).ToString());
        }

        [Test]
        public void ShouldGenerateBaseUriWithTerminatingBackslashFromSuppliedAbsoluteUri()
        {
            var uriFactory = new UriFactoryWorker("rooms", "{roomId}");
            Assert.AreEqual(new Uri("http://restinpractice.com:8080/uk/"), uriFactory.CreateBaseUri(new Uri("http://restinpractice.com:8080/uk/rooms/1234")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Supplied URI does not contain route prefix. Uri: [http://restinpractice.com:8080/uk/encounters/1234], Expected route prefix: [rooms].")]
        public void ThrowsExceptionWhenSuppliedUriDoesNotContainRoutePrefix()
        {
            var uriFactory = new UriFactoryWorker("rooms", "{roomId}");
            uriFactory.CreateBaseUri(new Uri("http://restinpractice.com:8080/uk/encounters/1234"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsNull()
        {
            new UriFactoryWorker(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsEmpty()
        {
            new UriFactoryWorker(string.Empty);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsWhitespace()
        {
            new UriFactoryWorker(" ");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: uriTemplateValue")]
        public void ThrowsExceptionIfUriTemplateIsNull()
        {
            new UriFactoryWorker("rooms", null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: uriTemplateValue")]
        public void ThrowsExceptionIfUriTemplateValueIsWhitespace()
        {
            new UriFactoryWorker("rooms", " ");
        }
    }
}