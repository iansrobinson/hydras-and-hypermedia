using System.Net.Http;
using System.Net.Http.Headers;
using RestInPractice.MediaTypes;

namespace RestInPractice.Exercises.Helpers
{
    public static class AtomClient
    {
        public static HttpClient CreateDefault()
        {
            var clientChannel = new HttpClientChannel {AllowAutoRedirect = true};
            var client = new HttpClient{ Channel = clientChannel};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value.MediaType));
            return client;
        }

        public static HttpClient CreateWithStubEndpoint(StubEndpoint endpoint)
        {
            var client = new HttpClient {Channel = endpoint};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AtomMediaType.Value.MediaType));
            return client;
        }
    }
}