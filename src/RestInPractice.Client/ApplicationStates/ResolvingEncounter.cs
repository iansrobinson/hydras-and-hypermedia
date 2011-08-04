using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class ResolvingEncounter : IApplicationState
    {
        public ResolvingEncounter(HttpResponseMessage currentResponse)
        {
        }

        public IApplicationState NextState(HttpClient client)
        {
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }
    }
}