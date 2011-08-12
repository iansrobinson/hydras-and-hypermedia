using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace RestInPractice.Client.Xhtml
{
    public class Form
    {
        public static Form ParseFromFeedExtension(SyndicationFeed feed)
        {
            var atomExtension = feed.ElementExtensions.FirstOrDefault(e => e.OuterNamespace.Equals(XhtmlNamespace.NamespaceName));
            if (atomExtension == null)
            {
                throw new ArgumentException("Feed does not contain XHTML form extension.", "feed");
            }
            return Parse(atomExtension);
        }

        public static Form ParseFromEntryContent(SyndicationItem entry)
        {
            if (entry.Content == null || !entry.Content.Type.Equals("xhtml"))
            {
                throw new ArgumentException("Entry does not contain XHTML form content.", "entry");
            }
            return Parse(((TextSyndicationContent)entry.Content).Text);
        }
        
        public static Form Parse(SyndicationElementExtension atomExtension)
        {
            return Parse(atomExtension.GetReader().ReadOuterXml());
        }
        
        public static Form Parse(string xhtml)
        {
            var doc = XDocument.Parse(xhtml);

            var controlData = GetControlData(doc);
            var textInputFields = GetTextInputData(doc);

            return new Form(controlData.Action, controlData.Method, controlData.Enctype, textInputFields.ToArray());
        }

        public static XNamespace XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private readonly Uri action;
        private readonly HttpMethod method;
        private readonly string enctype;
        private readonly IEnumerable<TextInput> textInputFields;

        public Form(string action, string method, string enctype, params TextInput[] textInputFields)
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

        public HttpRequestMessage CreateRequest(Uri baseUri)
        {
            var content = new FormUrlEncodedContent(textInputFields.Select(f => new KeyValuePair<string, string>(f.Name, f.Value)));
            content.Headers.ContentType = new MediaTypeHeaderValue(enctype);
            var request = new HttpRequestMessage{Method = method, RequestUri = new Uri(baseUri, action), Content = content};
            return request;
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