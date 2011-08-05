using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestInPractice.Client
{
    public class ApplicationStateInfo
    {
        private readonly IEnumerable<Uri> history;

        public ApplicationStateInfo(IEnumerable<Uri> history)
        {
            this.history = new List<Uri>(history).AsReadOnly();
        }

        public IEnumerable<Uri> History
        {
            get { return history; }
        }
    }
}
