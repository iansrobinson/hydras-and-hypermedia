using System.Net.Http;

namespace RestInPractice.Client
{
    public interface IApplicationState
    {
        IApplicationState NextState(HttpClient client);
        HttpResponseMessage CurrentResponse { get; }
        ApplicationStateInfo ApplicationStateInfo { get; }
        bool IsTerminalState { get; }
    }
}