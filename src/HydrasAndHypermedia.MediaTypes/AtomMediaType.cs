using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace HydrasAndHypermedia.MediaTypes
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
            SupportedMediaTypes.Add(Value);
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            stream.Seek(0, SeekOrigin.Begin);

            if (type.Equals(typeof(SyndicationItem)))
            {
                var entryFormatter = new Atom10ItemFormatter();
                entryFormatter.ReadFrom(XmlReader.Create(stream));
                return entryFormatter.Item;
            }

            if (type.Equals(typeof(SyndicationFeed)))
            {
                var feedFormatter = new Atom10FeedFormatter();
                feedFormatter.ReadFrom(XmlReader.Create(stream));
                return feedFormatter.Feed;
            }

            throw new InvalidOperationException("Expected to be called with type SyndicationItem or SyndicationFeed.");
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            if (type.Equals(typeof(SyndicationItem)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    var itemFormatter = new Atom10ItemFormatter((SyndicationItem)value);
                    itemFormatter.WriteTo(writer);
                    writer.Flush();
                }
            }
            else if (type.Equals(typeof(SyndicationFeed)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    var feedFormatter = new Atom10FeedFormatter((SyndicationFeed)value);
                    feedFormatter.WriteTo(writer);
                    writer.Flush();
                }
            }
            else
            {
                throw new InvalidOperationException("Expected to be called with type SyndicationItem or SyndicationFeed.");
            }
        }
    }
}