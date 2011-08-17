using System;
using System.Net.Http;
using HydrasAndHypermedia.Server.Xhtml;
using NUnit.Framework;

namespace Tests.HydrasAndHypermedia.Server.Xhtml
{
    [TestFixture]
    public class FormWriterTests
    {
        [Test]
        public void ShouldReturnXhtmlRepresentationOfForm()
        {
            const string expectedXhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""POST"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""field1"" value=""field1value"" />
    <input type=""text"" name=""field2"" />
    <input type=""text"" name=""field3"" value="""" />
    <input type=""submit"" value=""Submit"" />
  </form>
</div>";

            var writer = new FormWriter(new Uri("/encounters/1", UriKind.Relative), HttpMethod.Post, new TextInput("field1", "field1value"), new TextInput("field2"), new TextInput("field3", string.Empty));

            Assert.AreEqual(expectedXhtml, writer.ToXhtml());
        }
    }
}