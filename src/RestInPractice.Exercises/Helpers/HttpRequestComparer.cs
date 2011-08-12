using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Exercises.Helpers
{
    public class HttpRequestComparer : IEqualityComparer<HttpRequestMessage>
    {
        public static readonly IEqualityComparer<HttpRequestMessage> Instance = new HttpRequestComparer();

        private HttpRequestComparer()
        {
        }

        public bool Equals(HttpRequestMessage x, HttpRequestMessage y)
        {
            var result = true;
            result &= x.RequestUri.Equals(y.RequestUri);
            result &= x.Method.Equals(y.Method);
            result &= x.Content.Headers.ContentType.Equals(y.Content.Headers.ContentType);
            result &= x.Content.ReadAsString().Equals(y.Content.ReadAsString());
            return result;
        }

        public int GetHashCode(HttpRequestMessage obj)
        {
            return obj.GetHashCode();
        }
    }
}