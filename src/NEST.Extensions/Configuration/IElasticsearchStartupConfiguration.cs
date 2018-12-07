using System.Threading.Tasks;
using Nest;

namespace NEST.Extensions.Configuration
{
    public interface IElasticsearchStartupConfiguration
    {
        Task Configure(IElasticClient client);
    }
}