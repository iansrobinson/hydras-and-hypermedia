﻿using System;
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

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };

        private AtomMediaType()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Value));
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
                    var feedFormatter = new Atom10FeedFormatter((SyndicationFeed) value);
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