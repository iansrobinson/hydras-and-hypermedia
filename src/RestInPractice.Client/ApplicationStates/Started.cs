using System;
using System.Net.Http;
using HydrasAndHypermedia.Client.ApplicationStates;

namespace HydrasAndHypermedia.Client.ApplicationStates
{
    public class Started : IApplicationState
    {
        private readonly Uri entryPointUri;
        private readonly ApplicationStateInfo applicationStateInfo;

        public Started(Uri entryPointUri, ApplicationStateInfo applicationStateInfo)
        {
            this.entryPointUri = entryPointUri;
            this.applicationStateInfo = applicationStateInfo;
        }

        public IApplicationState NextState(HttpClient client)
        {
            return new Exploring(client.Get(entryPointUri), applicationStateInfo.GetBuilder().AddToHistory(entryPointUri).Build());
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
            get { return false; }
        }
    }
}