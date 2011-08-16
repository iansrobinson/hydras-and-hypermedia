using System;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class Defeated : IApplicationState
    {
        private readonly HttpResponseMessage currentResponse;
        private readonly ApplicationStateInfo applicationStateInfo;

        public Defeated(HttpResponseMessage currentResponse, ApplicationStateInfo applicationStateInfo)
        {
            this.currentResponse = currentResponse;
            this.applicationStateInfo = applicationStateInfo;
        }

        public IApplicationState NextState(HttpClient client)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage CurrentResponse
        {
            get { return currentResponse; }
        }

        public ApplicationStateInfo ApplicationStateInfo
        {
            get { return applicationStateInfo; }
        }

        public bool IsTerminalState
        {
            get { return true; }
        }
    }
}