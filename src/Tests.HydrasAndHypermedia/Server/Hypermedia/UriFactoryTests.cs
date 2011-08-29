using System;
using System.Collections.Generic;
using NUnit.Framework;
using HydrasAndHypermedia.Server.Hypermedia;

namespace Tests.HydrasAndHypermedia.Server.Hypermedia
{
    [TestFixture]
    public class UriFactoryTests
    {
        [Test]
        public void ShouldAllowRegistrationByPassingGenericParameterToRegisterMethod()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Treasure>();
            uriFactory.Register<Monster>();

            Assert.AreEqual(new Uri("http://restinpractice.com/treasures/1234"), uriFactory.CreateAbsoluteUri<Treasure>(new Uri("http://restinpractice.com"), 1234));
            Assert.AreEqual(new Uri("monsters/1234", UriKind.Relative), uriFactory.CreateRelativeUri<Monster>(1234));
            Assert.AreEqual(new Uri("http://restinpractice.com/"), uriFactory.CreateBaseUri<Treasure>(new Uri("http://restinpractice.com/treasures/1234")));
        }

        [Test]
        public void ShouldAllowRegistrationByPassingTypeToRegisterMethod()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register(typeof(Treasure));
            uriFactory.Register(typeof(Monster));

            Assert.AreEqual(new Uri("http://restinpractice.com/treasures/1234"), uriFactory.CreateAbsoluteUri<Treasure>(new Uri("http://restinpractice.com"), 1234));
            Assert.AreEqual(new Uri("monsters/1234", UriKind.Relative), uriFactory.CreateRelativeUri<Monster>(1234));
            Assert.AreEqual(new Uri("http://restinpractice.com/"), uriFactory.CreateBaseUri<Treasure>(new Uri("http://restinpractice.com/treasures/1234")));
        }
        
        [Test]
        public void ShouldCreateBaseUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/"), uriFactory.CreateBaseUri<Monster>(new Uri("http://localhost:8080/virtual-directory/monsters/1")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateBaseUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateBaseUri<Monster>(new Uri("http://localhost:8080/virtual-directory/monsters/1"));
        }

        [Test]
        public void ShouldCreateAbsoluteUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/monsters/1"), uriFactory.CreateAbsoluteUri<Monster>(new Uri("http://localhost:8080/virtual-directory/"), "1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateAbsoluteUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateAbsoluteUri<Monster>(new Uri("http://localhost:8080/virtual-directory/"), "1");
        }

        [Test]
        public void ShouldCreateRelativeUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual(new Uri("monsters/1", UriKind.Relative), uriFactory.CreateRelativeUri<Monster>("1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateRelativeUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateRelativeUri<Monster>("1");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException))]
        public void ThrowsExceptionIfEntryAlreadyExistsForType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();
            uriFactory.Register<Monster>();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (UriTemplateMissingException))]
        public void ThrowsExceptionIfTypeIsNotAttributedWithUriTemplateAttribute()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<string>();
        }

        [Test]
        public void ShouldReturnRoutePrefixForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual("monsters", uriFactory.GetRoutePrefix<Monster>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetRoutePrefixForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetRoutePrefix<Monster>();
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValue<Monster>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetUriTemplateValueForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetUriTemplateValue<Monster>();
        }

        [Test]
        public void WhenPassingGuidAsUriTemplateParameterShouldRemoveAllDashes()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual(new Uri("monsters/00000000000000000000000000000000", UriKind.Relative), uriFactory.CreateRelativeUri<Monster>(Guid.Empty));
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Monster>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValueFor(typeof (Monster)));
        }

        [Test]
        [ExpectedException(typeof (KeyNotFoundException))]
        public void ThrowsExceptionWhenTryingToGetUriTemplateValueForTypeThatHasNotBeenRegistered()
        {
            var uriFactory = new UriFactory();

            uriFactory.GetUriTemplateValueFor(typeof (Monster));
        }

        [UriTemplate("monsters", "{id}")]
        private class Monster
        {
        }

        [UriTemplate("treasures", "{id}")]
        private class Treasure
        {
        }
    }
}