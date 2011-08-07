using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RestInPractice.Client.Xhtml
{
    public class FormReader
    {
        public static FormReader Read(string xhtml)
        {
            var doc = XDocument.Parse(xhtml);

            var controlData = GetControlData(doc);
            var textInputFields = GetTextInputData(doc);

            return new FormReader(controlData.Action, controlData.Method, controlData.Enctype, textInputFields.ToArray());
        }

        public static XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private readonly Uri action;
        private readonly HttpMethod method;
        private readonly string enctype;
        private readonly IEnumerable<TextInput> textInputFields;

        public FormReader(string action, string method, string enctype, params TextInput[] textInputFields)
        {
            this.action = new Uri(action, UriKind.RelativeOrAbsolute);
            this.method = new HttpMethod(method.ToUpper());
            this.enctype = enctype;
            this.textInputFields = new List<TextInput>(textInputFields).AsReadOnly();
        }

        public Uri Action
        {
            get { return action; }
        }

        public HttpMethod Method
        {
            get { return method; }
        }

        public string Enctype
        {
            get { return enctype; }
        }

        public IEnumerable<TextInput> TextInputFields
        {
            get { return textInputFields; }
        }

        private static dynamic GetControlData(XContainer doc)
        {
            return (from form in doc.Descendants(XhtmlNamespace + "form")
                    let action = form.Attribute("action")
                    let method = form.Attribute("method")
                    let enctype = form.Attribute("enctype")
                    where action != null
                    select new {Action = action.Value, Method = method.Value, Enctype = enctype.Value}).FirstOrDefault();
        }

        private static IEnumerable<TextInput> GetTextInputData(XContainer doc)
        {
            return (from input in doc.Descendants(XhtmlNamespace + "input")
                    let type = input.Attribute("type")
                    let name = input.Attribute("name")
                    let value = input.Attribute("value")
                    where type.Value.Equals("text")
                    select new TextInput(name.Value, value == null ? null : value.Value));
        }
    }

    public static class TextInputFieldsExtensions
    {
        public static TextInput Single(this IEnumerable<TextInput> fields, string name)
        {
            return fields.FirstOrDefault(f => f.Name.Equals(name));
        }
    }
}