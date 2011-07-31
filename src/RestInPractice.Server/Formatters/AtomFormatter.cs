﻿using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Server.Formatters
{
    public class AtomFormatter : MediaTypeFormatter
    {
        public static readonly MediaTypeFormatter Instance = new AtomFormatter();
        
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates};

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