using System;
using System.Collections.Generic;
using System.Linq;

namespace HydrasAndHypermedia.Client
{
    public class ApplicationStateInfo
    {
        public static ApplicationStateInfo WithEndurance(int endurance)
        {
            return new ApplicationStateInfo(endurance, new Uri[]{});
        }
        
        private readonly int endurance;
        private readonly IEnumerable<Uri> history;

        private ApplicationStateInfo(int endurance, IEnumerable<Uri> history)
        {
            this.endurance = endurance;
            this.history = new List<Uri>(history).AsReadOnly();
        }

        public int Endurance
        {
            get { return endurance; }
        }

        public IEnumerable<Uri> History
        {
            get { return history; }
        }

        public IApplicationStateInfoBuilder GetBuilder()
        {
            return new ApplicationStateInfoBuilder(this);
        }

        private class ApplicationStateInfoBuilder : IApplicationStateInfoBuilder
        {
            private int endurance;
            private IEnumerable<Uri> history;

            public ApplicationStateInfoBuilder(ApplicationStateInfo info)
            {
                endurance = info.Endurance;
                history = info.history;
            }

            public IApplicationStateInfoBuilder UpdateEndurance(int newValue)
            {
                endurance = newValue;
                return this;
            }

            public IApplicationStateInfoBuilder AddToHistory(params Uri[] uris)
            {
                history = history.Concat(uris);
                return this;
            }

            public ApplicationStateInfo Build()
            {
                return new ApplicationStateInfo(endurance, history);
            }
        }
    }
}