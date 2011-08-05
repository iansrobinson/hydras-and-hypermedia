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
    <input type=""text"" name=""field1"" value=""field1value""/>
    <input type=""text"" name=""field2""/>
  </form>
</div>";

        [Test]
        public void ShouldParseActionFromForm()
        {
            var form = new Form(Xhtml);
            Assert.AreEqual("/encounters/1", form.Action);
        }

        [Test]
        public void ShouldParseMethodFromForm()
        {
            var form = new Form(Xhtml);
            Assert.AreEqual("post", form.Method);
        }

        [Test]
        public void ShouldParseEnctypeFromForm()
        {
            var form = new Form(Xhtml);
            Assert.AreEqual("application/x-www-form-urlencoded", form.Enctype);
        }

        [Test]
        public void ShouldParseAllTextInputFieldsFromForm()
        {
            var form = new Form(Xhtml);
            Assert.AreEqual(2, form.TextInputFields.Count());
        }

        [Test]
        public void ShouldParseTextInputFieldWithValue()
        {
            var form = new Form(Xhtml);
            var field1 = form.TextInputFields.First(f => f.Name.Equals("field1"));

            Assert.AreEqual("field1value", field1.Value);
        }

        [Test]
        public void ShouldParseTextInputFieldWithoutValue()
        {
            var form = new Form(Xhtml);
            var field2 = form.TextInputFields.First(f => f.Name.Equals("field2"));

            Assert.AreEqual(string.Empty, field2.Value);
        }
    }
}