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
        private const string MediaType = "application/atom+xml";
        private static readonly MediaTypeFormatter Instance;
        private static readonly MediaTypeHeaderValue DefaultValue;
        private static readonly MediaTypeHeaderValue FeedValue;
        private static readonly MediaTypeHeaderValue EntryValue;
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };

        static AtomMediaType()
        {
            DefaultValue = new MediaTypeHeaderValue(MediaType);
            FeedValue = new MediaTypeHeaderValue(MediaType);
            EntryValue = new MediaTypeHeaderValue(MediaType);
            FeedValue.Parameters.Add(new NameValueHeaderValue("type", "feed"));
            FeedValue.CharSet = "utf-8";
            EntryValue.Parameters.Add(new NameValueHeaderValue("type", "entry"));
            EntryValue.CharSet = "utf-8";
            Instance = new AtomMediaType();
        }

        public static MediaTypeFormatter Formatter
        {
            get { return Instance; }
        }

        public static MediaTypeHeaderValue Value
        {
            get { return DefaultValue; }
        }

        public static MediaTypeHeaderValue Feed
        {
            get { return FeedValue; }
        }

        public static MediaTypeHeaderValue Entry
        {
            get { return EntryValue; }
        }

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