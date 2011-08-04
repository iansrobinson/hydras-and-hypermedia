using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.MediaTypes
{
    public class AtomMediaType : MediaTypeFormatter
    {
        public static readonly MediaTypeFormatter Formatter = new AtomMediaType();
        public const String Value = "application/atom+xml";

        public static MediaTypeHeaderValue FeedValue
        {
            get
            {
                var header = new MediaTypeHeaderValue(Value);
                header.Parameters.Add(new NameValueHeaderValue("type", "feed"));
                return header;
            }
        }

        public static MediaTypeHeaderValue EntryValue
        {
            get
            {
                var header = new MediaTypeHeaderValue(Value);
                header.Parameters.Add(new NameValueHeaderValue("type", "entry"));
                return header;
            }
        }

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

        private AtomMediaType()
        {
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            throw new NotImplementedException();
        }
    }
}