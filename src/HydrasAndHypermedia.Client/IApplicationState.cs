using System.Net.Http;

namespace HydrasAndHypermedia.Client
{
    public interface IApplicationState
    {
        IApplicationState NextState(HttpClient client);
        HttpResponseMessage CurrentResponse { get; }
        ApplicationStateInfo ApplicationStateInfo { get; }
        bool IsTerminalState { get; }
    }
}