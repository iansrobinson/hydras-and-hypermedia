using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class Error : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;
        private readonly IEnumerable<Uri> history;

        public Error(HttpResponseMessage currentResponse, IEnumerable<Uri> history)
        {
            this.currentResponse = currentResponse;
            this.history = new List<Uri>(history).AsReadOnly();
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
            get { return history; }
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}