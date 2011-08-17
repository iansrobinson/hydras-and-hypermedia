using System.IO;
using System.Net.Http;
using System.Threading;

namespace HydrasAndHypermedia.Exercises.Helpers
{
    public class MockEndpoint : HttpClientChannel
    {
        private readonly HttpResponseMessage response;
        private HttpRequestMessage receivedRequest;

        public MockEndpoint(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            receivedRequest = new HttpRequestMessage {Method = request.Method, RequestUri = request.RequestUri, Version = request.Version};
            foreach (var h in request.Headers)
            {
                receivedRequest.Headers.Add(h.Key, h.Value);
            }

            if (request.Content != null)
            {
                var stream = new MemoryStream();
                request.Content.CopyTo(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var content = new StreamContent(stream);
                foreach (var h in request.Content.Headers)
                {
                    content.Headers.Add(h.Key, h.Value);
                }

                receivedRequest.Content = content;
            }

            return response;
        }

        public HttpRequestMessage ReceivedRequest
        {
            get { return receivedRequest; }
        }
    }
}