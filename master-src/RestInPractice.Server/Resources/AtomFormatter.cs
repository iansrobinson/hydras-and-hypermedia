using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Server.Resources
{
    public class AtomFormatter : MediaTypeFormatter
    {
        public const String Value = "application/atom+xml";
        public static readonly MediaTypeFormatter Formatter = new AtomFormatter();

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

        private AtomFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Value));
        }

        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            if (type.Equals(typeof (SyndicationItem)))
            {
                var entryFormatter = new Atom10ItemFormatter();
                entryFormatter.ReadFrom(XmlReader.Create(stream));
                return entryFormatter.Item;
            }

            if (type.Equals(typeof (SyndicationFeed)))
            {
                var feedFormatter = new Atom10FeedFormatter();
                feedFormatter.ReadFrom(XmlReader.Create(stream));
                return feedFormatter.Feed;
            }

            throw new InvalidOperationException("Expected to be called with type SyndicationItemFormatter or SyndicationFeedFormatter.");
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            if (type.Equals(typeof (SyndicationItem)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    ((Atom10ItemFormatter) value).WriteTo(writer);
                    writer.Flush();
                }
            }
            else if (type.Equals(typeof (SyndicationFeed)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    ((Atom10FeedFormatter) value).WriteTo(writer);
                    writer.Flush();
                }
            }
            else
            {
                throw new InvalidOperationException("Expected to be called with type SyndicationItemFormatter or SyndicationFeedFormatter.");
            }
        }
    }
}