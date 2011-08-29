using System;
using HydrasAndHypermedia.Server.Hypermedia;
using NUnit.Framework;

namespace Tests.HydrasAndHypermedia.Server.Hypermedia
{
    [TestFixture]
    public class CheckStringTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: p")]
        public void ThrowsExceptionWhenCheckingForNullAndArgIsNull()
        {
            CheckString.Is(Not.Null, null, "p");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: p")]
        public void ThrowsExceptionWhenCheckingForEmptyAndArgIsEmpty()
        {
            CheckString.Is(Not.Null | Not.Empty, string.Empty, "p");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: p")]
        public void ThrowsExceptionWhenCheckingForWhitespaceAndArgIsWhitespace()
        {
            CheckString.Is(Not.Null | Not.Empty | Not.Whitespace, " ", "p");
        }

        [Test]
        public void DoesNotThrowExceptionWhenCheckingForWhitespaceAndArgIsEmpty()
        {
            CheckString.Is(Not.Whitespace, string.Empty, "p");
        }

        [Test]
        public void DoesNotThrowExceptionWhenCheckingForWhitespaceAndArgIsNull()
        {
            CheckString.Is(Not.Whitespace, null, "p");
        }

        [Test]
        public void DoesNotThrowExceptionWhenCheckingForEmptyAndArgIsWhitespace()
        {
            CheckString.Is(Not.Empty, " ", "p");
        }

        [Test]
        public void DoesNotThrowExceptionWhenCheckingForEmptyAndArgIsNull()
        {
            CheckString.Is(Not.Empty, null, "p");
        }
    }
}