using Google.Api;
using Microsoft.Extensions.Configuration;
using PubSub.Net;
using PubSub.Net.Interfaces;
using Test.PubSub.Net.WebApi.Pubub;

namespace Test.PubSub.Net.WebApi
{
    public static class DI
    {
        public static IServiceCollection AddPubSub(this IServiceCollection services, IConfiguration configuration)
        {
            var pubSubConfig = configuration.GetSection("*section name*").Get<PubSubConfiguration>();

            services.AddSingleton<IPubSubInfraConfig, PubSubConfiguration>(_ =>
                configuration.GetSection(nameof(PubSubConfiguration)).Get<PubSubConfiguration>());

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly
                                                                                        .GetTypes()
                                                                                        .Where(t => typeof(IPipelineMiddleware)
                                                                                        .IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)).ToList();

            types.ForEach(t => services.AddScoped(t));

            services.AddSingleton<IEventHandlerService, EventHandlerService>();

            services.AddScoped<IDbTransactionService, DbTransactionService>();

            services.AddScoped<TestSubscriber>();
            services.AddScoped<IPublisher, Publisher>();
            services.AddScoped<ISubscriberListener, SubscriberListener>();

            var sp = services.BuildServiceProvider().CreateScope().ServiceProvider;

            var topology = new PubSubTopology(sp);

            topology.WithEmulator()
                    .WithMiddleware<PrePubTestProcess>();

            topology.AddSubsciber<TestSubscriber, TestEvent>("*enter topic name here*");

            services.AddSingleton<IPubSubTopology, PubSubTopology>(_ => topology);

            return services;
        }
    }
}
