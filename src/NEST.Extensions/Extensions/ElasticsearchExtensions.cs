using System;
using System.Text;
using System.Threading.Tasks;
using Nest;
using NEST.Extensions.Builders;
using NEST.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ElasticsearchExtensions
    {
        public static IElasticsearchBuilder AddElasticsearch(this IServiceCollection services, string connectionUrl, bool debugMode = false, bool prettyJson = true)
        {
            if (string.IsNullOrEmpty(connectionUrl)) throw new ArgumentNullException(nameof(connectionUrl), "Value cannot be null or empty.");

            var builder = new ElasticsearchBuilder(services);

            builder.ConnectionSettings = new ConnectionSettings(new Uri(connectionUrl))
                .PrettyJson(debugMode && prettyJson)
                .OnRequestCompleted(response =>
                {
                    if (debugMode)
                    {
                        if (response.RequestBodyInBytes != null)
                        {
                            Console.WriteLine($"{response.HttpMethod} {response.Uri} \n" + $"{Encoding.UTF8.GetString(response.RequestBodyInBytes)}");
                        }
                        else
                        {
                            Console.WriteLine($"{response.HttpMethod} {response.Uri}");
                        }

                        if (response.ResponseBodyInBytes != null)
                        {
                            Console.WriteLine($"Status: {response.HttpStatusCode}\n" + $"Body:{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}\n" + $"{new string('-', 30)}\n");
                        }
                        else
                        {
                            Console.WriteLine($"Status: {response.HttpStatusCode}\n" + $"{new string('-', 30)}\n");
                        }
                    }
                });

            if (debugMode)
            {
                builder.ConnectionSettings = builder.ConnectionSettings?.EnableDebugMode();
            }

            return builder;
        }

        /// <summary>
        /// Builds the IElasticClient instance with the connection settings.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IElasticsearchInstanceBuilder AddClient(this IElasticsearchBuilder builder)
        {
            if (builder.ConnectionSettings == null) throw new ArgumentNullException(nameof(builder.ConnectionSettings), "Connection Settings must be set.");

            builder.Services.AddSingleton<IElasticClient>(new ElasticClient(builder.ConnectionSettings));

            return new ElasticsearchInstanceBuilder(builder.Services);
        }

        /// <summary>
        /// Adds the mapping for a given type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IElasticsearchBuilder AddDefaultMappingFor<T>(this IElasticsearchBuilder builder, string name) where T : class
        {
            if (builder.ConnectionSettings == null) throw new ArgumentNullException(nameof(builder.ConnectionSettings), "Connection Settings must be set.");

            IElasticClient client = builder.Services.BuildServiceProvider().GetRequiredService(typeof(IElasticClient)) as IElasticClient;
            if (client != null)
            {
                throw new Exception($"IElasticClient has already been configured. Ensure '{nameof(AddDefaultMappingFor)}()' is called before '{nameof(AddClient)}()'");
            }

            builder.ConnectionSettings = builder.ConnectionSettings
               .DefaultMappingFor<T>(m => m
                   .TypeName(name)
                   .IndexName(name));

            return builder;
        }

        /// <summary>
        /// Creates an instance of IElasticsearchIndexConfiguration and executes the Configre method. 
        /// 
        /// The Configure method is called once on application start so this is good for setting up index creations or other checks.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public static IElasticsearchInstanceBuilder RunOnStartup<TService>(this IElasticsearchInstanceBuilder builder) where TService : class, IElasticsearchStartupConfiguration
        {
            IElasticClient client = builder.Services.BuildServiceProvider().GetRequiredService(typeof(IElasticClient)) as IElasticClient;
            if (client == null)
            {
                throw new Exception($"IElasticClient is not configured in services. Ensure .AddClient() is called before this.");
            }

            builder.Services.AddScoped<IElasticsearchStartupConfiguration, TService>();

            var config = builder.Services.BuildServiceProvider().GetRequiredService<IElasticsearchStartupConfiguration>();
            config.Configure(client).ConfigureAwait(false).GetAwaiter().GetResult();

            return builder;
        }
    }
}