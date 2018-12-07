using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace NEST.Extensions.Builders
{
    public class ElasticsearchBuilder : IElasticsearchBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticsearchBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public ElasticsearchBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public IServiceCollection Services { get; }

        public ConnectionSettings ConnectionSettings { get; set; }
    }
}