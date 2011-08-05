using System;
using System.Net.Http;

namespace RestInPractice.Client.ApplicationStates
{
    public class ResolvingEncounter : IApplicationState
    {
        public ResolvingEncounter(HttpResponseMessage currentResponse, ApplicationStateInfo applicationStateInfo)
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

        public ApplicationStateInfo ApplicationStateInfo
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTerminalState
        {
            get { throw new NotImplementedException(); }
        }
    }
}