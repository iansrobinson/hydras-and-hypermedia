using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RestInPractice.Client.Xhtml
{
    public class Form
    {
        public static XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private readonly string action;
        private readonly string method;
        private readonly string enctype;
        private readonly IEnumerable<TextInput> textInputFields;

        public Form(string xhtml)
        {
            var doc = XDocument.Parse(xhtml);

            var controlData = GetControlData(doc);
            action = controlData.Action;
            method = controlData.Method;
            enctype = controlData.Enctype;

            textInputFields = new List<TextInput>(GetTextInputData(doc)).AsReadOnly();
        }

        public string Action
        {
            get { return action; }
        }

        public string Method
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
                    select new TextInput(name.Value, value == null ? string.Empty : value.Value));
        }
    }
}