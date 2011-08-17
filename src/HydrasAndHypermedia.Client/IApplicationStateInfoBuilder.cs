using System;

namespace HydrasAndHypermedia.Client
{
    public interface IApplicationStateInfoBuilder
    {
        IApplicationStateInfoBuilder UpdateEndurance(int newValue);
        IApplicationStateInfoBuilder AddToHistory(params Uri[] uris);
        ApplicationStateInfo Build();
    }
}