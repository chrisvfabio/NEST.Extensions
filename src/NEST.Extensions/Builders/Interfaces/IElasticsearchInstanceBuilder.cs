using Nest;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IElasticsearchInstanceBuilder
    {
        IServiceCollection Services { get; }
        ConnectionSettings ConnectionSettings { get; set; }
    }
}