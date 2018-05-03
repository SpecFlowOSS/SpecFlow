using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;

namespace TechTalk.SpecFlow.Rpc.Server
{
    public interface IClientConnectionHost
    {
        Task<IClientConnection> CreateListenTask(CancellationToken cancellationToken, Logger logger);
    }
}