using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RestInPractice.Client.Xhtml
{
    public class Form
    {
        public static Form Parse(string xhtml)
        {
            var doc = XDocument.Parse(xhtml);

            var controlData = GetControlData(doc);
            var textInputFields = GetTextInputData(doc);

            return new Form(controlData.Action, controlData.Method, controlData.Enctype, textInputFields.ToArray());
        }

        public static XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true, IndentChars = "  ", OmitXmlDeclaration = true};

        private readonly string action;
        private readonly string method;
        private readonly string enctype;
        private readonly IEnumerable<TextInput> textInputFields;

        public Form(string action, string method, string enctype, params TextInput[] textInputFields)
        {
            this.action = action;
            this.method = method;
            this.enctype = enctype;
            this.textInputFields = new List<TextInput>(textInputFields).AsReadOnly();
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

        public string ToXhtml()
        {
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb, XmlWriterSettings);
            writer.WriteStartElement("div", XhtmlNamespace.NamespaceName);
            writer.WriteStartElement("form");
            writer.WriteAttributeString("action", action);
            writer.WriteAttributeString("method", method);
            writer.WriteAttributeString("enctype", enctype);

            foreach (var field in textInputFields)
            {
                writer.WriteStartElement("input");
                writer.WriteAttributeString("type", "text");
                writer.WriteAttributeString("name", field.Name);
                if (field.Value != null)
                {
                    writer.WriteAttributeString("value", field.Value);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            return sb.ToString();
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