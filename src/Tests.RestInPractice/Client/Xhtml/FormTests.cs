using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Xml;
using NUnit.Framework;
using RestInPractice.Client.Xhtml;

namespace Tests.RestInPractice.Client.Xhtml
{
    [TestFixture]
    public class FormTests
    {
        private const string Xhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""post"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""field1"" value=""field1value"" />
    <input type=""text"" name=""field2"" />
    <input type=""text"" name=""field3"" value="""" />
  </form>
</div>";

        private static readonly SyndicationElementExtension AtomExtension = new SyndicationElementExtension(XmlReader.Create(new StringReader(Xhtml)));

        [Test]
        public void ShouldParseActionFromForm()
        {            
            var reader = Form.Parse(Xhtml);
            Assert.AreEqual(new Uri("/encounters/1", UriKind.Relative), reader.Action);
        }

        [Test]
        public void ShouldParseMethodFromForm()
        {
            var reader = Form.Parse(Xhtml);
            Assert.AreEqual(HttpMethod.Post, reader.Method);
        }

        [Test]
        public void ShouldParseEnctypeFromForm()
        {
            var form = Form.Parse(Xhtml);
            Assert.AreEqual("application/x-www-form-urlencoded", form.Enctype);
        }

        [Test]
        public void ShouldParseAllTextInputFieldsFromForm()
        {
            var form = Form.Parse(Xhtml);
            Assert.AreEqual(3, form.TextInputFields.Count());
        }

        [Test]
        public void ShouldParseTextInputFieldWithValue()
        {
            var form = Form.Parse(Xhtml);
            var field1 = form.TextInputFields.Single("field1");

            Assert.AreEqual("field1value", field1.Value);
        }

        [Test]
        public void ShouldParseTextInputFieldWithoutValue()
        {
            var form = Form.Parse(Xhtml);
            var field2 = form.TextInputFields.Single("field2");

            Assert.AreEqual(null, field2.Value);
        }

        [Test]
        public void ShouldParseTextInputFieldWithEmptyValue()
        {
            var form = Form.Parse(Xhtml);
            var field3 = form.TextInputFields.Single("field3");

            Assert.AreEqual(string.Empty, field3.Value);
        }

        [Test]
        public void ShouldParseFormFromSyndicationElementExtension()
        {
            var form = Form.Parse(AtomExtension);
            Assert.AreEqual(new Uri("/encounters/1", UriKind.Relative), form.Action);
        }

        [Test]
        public void ShouldParseFormFromFeedWithFeedExtension()
        {
            var feed = new SyndicationFeed();
            feed.ElementExtensions.Add(XmlReader.Create(new StringReader(Xhtml)));

            var form = Form.ParseFromFeedExtension(feed);
            Assert.AreEqual(new Uri("/encounters/1", UriKind.Relative), form.Action);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Feed does not contain XHTML form extension.\r\nParameter name: feed")]
        public void ThrowsExceptionIfFeedDoesNotContainXhtmlFeedExtension()
        {
            var feed = new SyndicationFeed();
            Form.ParseFromFeedExtension(feed);
        }

        [Test]
        public void ShouldParseFormFromEntryWithXhtmlContent()
        {
            var entry = new SyndicationItem {Content = SyndicationContent.CreateXhtmlContent(Xhtml)};

            var form = Form.ParseFromEntryContent(entry);
            Assert.AreEqual(new Uri("/encounters/1", UriKind.Relative), form.Action);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Entry does not contain XHTML form content.\r\nParameter name: entry")]
        public void ThrowsExceptionIfEntryDoesNotContainContent()
        {
            var entry = new SyndicationItem();
            Form.ParseFromEntryContent(entry);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Entry does not contain XHTML form content.\r\nParameter name: entry")]     
        public void ThrowsExceptionIfEntryDoesNotContainXhtmlFormContent()
        {
            var entry = new SyndicationItem { Content = SyndicationContent.CreatePlaintextContent("") };
            Form.ParseFromEntryContent(entry);
        }
    }
}