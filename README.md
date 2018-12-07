# NEST.Extensions

NEST Extensions provides an IoC way of accessing the ElasticClient instance through your application. It also allows you to easily define default mapping types and a startup configuration instance for handling things like creating indicies, seeding data and more on application start.

## Installation

```sh
dotnet add package NEST.Extensions
```

## Usage


```c#
public void ConfigureServices(IServiceCollection services)
{
    // other services...

    services.AddElasticsearch("http://127.0.0.1:9200")
        .AddDefaultMappingFor<Dog>("dogs")
        .AddDefaultMappingFor<Cat>("cats")
        .AddClient()
        .RunOnStartup<MyElasticsearchStartupConfiguration>();
}
```


> Startup Configuration handler

```c#
public class MyElasticsearchStartupConfiguration : IElasticsearchStartupConfiguration
{
    public Task Configure(IElasticClient client)
    {
        // Start up checks i.e. Index creations, seeding, etc.
        if (!client.IndexExists("dogs").IsValid)
        {
            client.CreateIndex("dogs", new ElasticsearchIndex("dogs", c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                )
                .Mappings(m => m
                    .Map<Dog>(d => d
                        .AutoMap()
                    )
            )));
        }

        return Task.CompletedTask;
    }
}
```


## Roadmap

- Better Configure handler
- Experiment with Migrations