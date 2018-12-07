using Nest;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IElasticsearchBuilder
    {
        IServiceCollection Services { get; }

        ConnectionSettings ConnectionSettings { get; set; }
    }
}