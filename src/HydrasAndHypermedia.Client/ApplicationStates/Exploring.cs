﻿using System;
using System.Net.Http;

namespace HydrasAndHypermedia.Client.ApplicationStates
{
    public class Exploring : IApplicationState
    {
        public Exploring(HttpResponseMessage currentResponse, ApplicationStateInfo applicationStateInfo)
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