using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Server.Formatters
{
    public class AtomFormatter : MediaTypeFormatter
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };
        
        public AtomFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/atom+xml"));
        }
        
        public override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
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
            
            throw new InvalidOperationException();
        }

        public override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            if (type.Equals(typeof(SyndicationItem)))
            {
                var entryFormatter = new Atom10ItemFormatter((SyndicationItem)value);
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    entryFormatter.WriteTo(writer);
                    writer.Flush();
                }
            }
            else if (type.Equals(typeof(SyndicationFeed)))
            {
                var feedFormatter = new Atom10FeedFormatter((SyndicationFeed)value);
                using (var writer = XmlWriter.Create(stream, WriterSettings))
                {
                    feedFormatter.WriteTo(writer);
                    writer.Flush();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}