using System;
using System.Net.Http;
using System.Threading;

namespace RestInPractice.Exercises.Helpers
{
    public class StubEndpoint : HttpClientChannel
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> generateResponse;

        public StubEndpoint(Func<HttpRequestMessage, HttpResponseMessage> generateResponse)
        {
            this.generateResponse = generateResponse;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return generateResponse(request);
        }
    }
}