using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestInPractice.Client
{
    public interface IApplicationState
    {
        IApplicationState NextState(HttpClient client);
        HttpResponseMessage CurrentResponse { get; }
        IEnumerable<Uri> History { get; }
        bool IsTerminalState { get; }
    }
}