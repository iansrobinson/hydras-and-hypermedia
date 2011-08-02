using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class Started : IApplicationState
    {
        private readonly Uri entryPointUri;

        public Started(Uri entryPointUri)
        {
            this.entryPointUri = entryPointUri;
        }

        public IApplicationState NextState(HttpClient client)
        {
            return new Exploring(client.Get(entryPointUri));
        }

        public HttpResponseMessage CurrentResponse
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<Uri> History
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTerminalState
        {
            get { return false; }
        }
    }
}