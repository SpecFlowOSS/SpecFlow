using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.IdeIntegration.Tracing
{
    public interface IIdeTracer
    {
        void Trace(string message, string category);
        bool IsEnabled(string category);
    }
}
