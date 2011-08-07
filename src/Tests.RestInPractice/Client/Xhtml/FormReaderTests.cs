using System.Linq;
using NUnit.Framework;
using RestInPractice.Client.Xhtml;

namespace Tests.RestInPractice.Client.Xhtml
{
    [TestFixture]
    public class FormReaderTests
    {
        private const string Xhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""post"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""field1"" value=""field1value"" />
    <input type=""text"" name=""field2"" />
    <input type=""text"" name=""field3"" value="""" />
  </form>
</div>";

        [Test]
        public void ShouldParseActionFromForm()
        {
            var reader = FormReader.Read(Xhtml);
            Assert.AreEqual("/encounters/1", reader.Action);
        }

        [Test]
        public void ShouldParseMethodFromForm()
        {
            var reader = FormReader.Read(Xhtml);
            Assert.AreEqual("post", reader.Method);
        }

        [Test]
        public void ShouldParseEnctypeFromForm()
        {
            var reader = FormReader.Read(Xhtml);
            Assert.AreEqual("application/x-www-form-urlencoded", reader.Enctype);
        }

        [Test]
        public void ShouldParseAllTextInputFieldsFromForm()
        {
            var reader = FormReader.Read(Xhtml);
            Assert.AreEqual(3, reader.TextInputFields.Count());
        }

        [Test]
        public void ShouldParseTextInputFieldWithValue()
        {
            var form = FormReader.Read(Xhtml);
            var field1 = form.TextInputFields.Single("field1");

            Assert.AreEqual("field1value", field1.Value);
        }

        [Test]
        public void ShouldParseTextInputFieldWithoutValue()
        {
            var reader = FormReader.Read(Xhtml);
            var field2 = reader.TextInputFields.Single("field2");

            Assert.AreEqual(null, field2.Value);
        }

        [Test]
        public void ShouldParseTextInputFieldWithEmptyValue()
        {
            var reader = FormReader.Read(Xhtml);
            var field3 = reader.TextInputFields.Single("field3");

            Assert.AreEqual(string.Empty, field3.Value);
        }

        [Test]
        public void ShouldReturnXhtmlRepresentationOfForm()
        {
            var reader = FormReader.Read(Xhtml);

            Assert.AreEqual(Xhtml, reader.ToXhtml());
        }

        [Test]
        public void ShouldReturnXhtmlRepresentationOfModifiedForm()
        {
            const string expectedXhtml = @"<div xmlns=""http://www.w3.org/1999/xhtml"">
  <form action=""/encounters/1"" method=""post"" enctype=""application/x-www-form-urlencoded"">
    <input type=""text"" name=""field1"" value=""field1value"" />
    <input type=""text"" name=""field2"" value=""field2value"" />
    <input type=""text"" name=""field3"" value="""" />
  </form>
</div>";
            
            var reader = FormReader.Read(Xhtml);
            reader.TextInputFields.Single("field2").Value = "field2value";

            Assert.AreEqual(expectedXhtml, reader.ToXhtml());
        }
    }
}