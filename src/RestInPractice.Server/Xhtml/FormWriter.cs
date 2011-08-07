using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RestInPractice.Server.Xhtml
{
    public class FormWriter
    {
        public static XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";
        private const string FormUrlEncoded = "application/x-www-form-urlencoded";
        private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true, IndentChars = "  ", OmitXmlDeclaration = true};

        private readonly Uri action;
        private readonly HttpMethod method;
        private readonly IEnumerable<TextInput> textInputFields;

        public FormWriter(Uri action, HttpMethod method, params TextInput[] textInputFields)
        {
            this.action = action;
            this.method = method;
            this.textInputFields = textInputFields;
        }

        public string ToXhtml()
        {
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb, XmlWriterSettings);
            writer.WriteStartElement("div", XhtmlNamespace.NamespaceName);
            writer.WriteStartElement("form");
            writer.WriteAttributeString("action", action.ToString());
            writer.WriteAttributeString("method", method.ToString());
            writer.WriteAttributeString("enctype", FormUrlEncoded);

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

            writer.WriteStartElement("input");
            writer.WriteAttributeString("type", "submit");
            writer.WriteAttributeString("value", "Submit");
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            return sb.ToString();
        }
    }
}