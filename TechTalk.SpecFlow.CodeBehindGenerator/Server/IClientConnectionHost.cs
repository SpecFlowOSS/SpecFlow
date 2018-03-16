using System.Threading;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.CodeBehindGenerator.Server
{
    public interface IClientConnectionHost
    {
        Task<IClientConnection> CreateListenTask(CancellationToken cancellationToken);
    }
}