using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Infrastructure
{
    /// <summary>
    /// This interface will be replaced in .Net Standard 2.1 by Microsoft's one
    /// </summary>
    public interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
