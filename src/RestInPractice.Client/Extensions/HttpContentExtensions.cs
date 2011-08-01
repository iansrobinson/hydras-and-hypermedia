using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace RestInPractice.Client.Extensions
{
    public static class HttpContentExtensions
    {
        //Hack to compensate for the fact that in Preview 4 of the Web APIs,
        //ReadAs<T> currently fails with ObjectDisposedException if called multiple times
        public static T ReadAsObject<T>(this HttpContent content, params MediaTypeFormatter[] formatters)
        {
            var contentType = content.Headers.ContentType.MediaType;
            var formatter = formatters.FirstOrDefault(f => f.SupportedMediaTypes.Any(m => m.MediaType.Equals(contentType, StringComparison.OrdinalIgnoreCase)));

            return (T) formatter.ReadFromStream(typeof (T), content.ContentReadStream, content.Headers);
        }
    }
}