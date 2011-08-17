using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace HydrasAndHypermedia.Client.Extensions
{
    public static class HttpContentExtensions
    {
        //Hack to compensate for the fact that in Preview 4 of the Web APIs,
        //ReadAs<T> currently fails with ObjectDisposedException if called multiple times
        public static T ReadAsObject<T>(this HttpContent content, params MediaTypeFormatter[] formatters)
        {
            var contentType = content.Headers.ContentType.MediaType;
            var formatter = formatters.FirstOrDefault(f => f.SupportedMediaTypes.Any(m => m.MediaType.Equals(contentType, StringComparison.OrdinalIgnoreCase)));

            if (formatter == null)
            {
                throw new InvalidOperationException("Unable to find formatter when reading object from content.");
            }

            return (T) formatter.ReadFromStream(typeof (T), content.ContentReadStream, content.Headers);
        }
    }
}