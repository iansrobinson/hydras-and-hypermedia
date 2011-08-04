using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class ResolvingEncounter : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;

        public ResolvingEncounter(HttpResponseMessage currentResponse)
        {
            this.currentResponse = currentResponse;
        }
        
        public IApplicationState NextState(HttpClient client)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
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