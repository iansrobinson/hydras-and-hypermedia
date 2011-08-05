using System.Linq;
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

        [Test]
        public void ShouldParseActionFromForm()
        {
            var form = Form.Parse(Xhtml);
            Assert.AreEqual("/encounters/1", form.Action);
        }

        [Test]
        public void ShouldParseMethodFromForm()
        {
            var form = Form.Parse(Xhtml);
            Assert.AreEqual("post", form.Method);
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
        public void ShouldReturnXhtmlRepresentationOfForm()
        {
            var form = Form.Parse(Xhtml);

            Assert.AreEqual(Xhtml, form.ToXhtml());
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
            
            var form = Form.Parse(Xhtml);
            form.TextInputFields.Single("field2").Value = "field2value";

            Assert.AreEqual(expectedXhtml, form.ToXhtml());
        }
    }
}