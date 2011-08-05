using System;
using NUnit.Framework;
using RestInPractice.Client.Xhtml;

namespace Tests.RestInPractice.Client.Xhtml
{
    [TestFixture]
    public class TextInputTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: name")]
        public void ThrowsExceptionIfNameIsNull()
        {
            new TextInput(null, "value");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be empty or whitespace.\r\nParameter name: name")]
        public void ThrowsExceptionIfNameIsEmpty()
        {
            new TextInput(string.Empty, "value");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be empty or whitespace.\r\nParameter name: name")]
        public void ThrowsExceptionIfNameIsWhitespace()
        {
            new TextInput(" ", "value");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: value")]
        public void ThrowsExceptionIfValueIsNull()
        {
            new TextInput("key", null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: value")]
        public void ThrowsExceptionIfValueIsWhitespace()
        {
            new TextInput("key", " ");
        }

        [Test]
        public void ValueCanBeEmpty()
        {
            var input = new TextInput("key", string.Empty);
            Assert.AreEqual(string.Empty, input.Value);
        }
    }
}