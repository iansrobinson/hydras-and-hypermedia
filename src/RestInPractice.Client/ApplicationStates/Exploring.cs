using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using RestInPractice.Client.Extensions;
using RestInPractice.MediaTypes;

namespace RestInPractice.Client.ApplicationStates
{
    public class Exploring : IApplicationState
    {
        public Exploring(HttpResponseMessage currentResponse) : this(currentResponse, new Uri[] { })
        {
        }

        public Exploring(HttpResponseMessage currentResponse, IEnumerable<Uri> history)
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