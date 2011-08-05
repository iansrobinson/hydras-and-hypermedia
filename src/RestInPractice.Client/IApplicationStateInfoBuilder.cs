using System;

namespace RestInPractice.Client
{
    public interface IApplicationStateInfoBuilder
    {
        IApplicationStateInfoBuilder UpdateEndurance(int newValue);
        IApplicationStateInfoBuilder AddToHistory(params Uri[] uris);
        ApplicationStateInfo Build();
    }
}