using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.MediaTypes
{
    public class AtomMediaType : MediaTypeFormatter
    {
        public const String Value = "application/atom+xml";
        public static readonly MediaTypeFormatter Instance = new AtomMediaType();
        
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };
        
        private AtomMediaType()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Value));
        }
        
        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            if (type.Equals(typeof(SyndicationItemFormatter)))
            {
                var entryFormatter = new Atom10ItemFormatter();
                entryFormatter.ReadFrom(XmlReader.Create(stream));
                return entryFormatter;
            }
            
            if (type.Equals(typeof(SyndicationFeedFormatter)))
            {
                var feedFormatter = new Atom10FeedFormatter();
                feedFormatter.ReadFrom(XmlReader.Create(stream));
                return feedFormatter;
            }

            throw new InvalidOperationException("Expected to be called with type SyndicationItemFormatter or SyndicationFeedFormatter.");
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            if (type.Equals(typeof(SyndicationItemFormatter)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    ((Atom10ItemFormatter)value).WriteTo(writer);
                    writer.Flush();
                }
            }
            else if (type.Equals(typeof(SyndicationFeedFormatter)))
            {
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    ((Atom10FeedFormatter)value).WriteTo(writer);
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