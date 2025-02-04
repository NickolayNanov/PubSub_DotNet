using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using PubSub.Net.Interfaces;

namespace PubSub.Net
{
    public class SubscriberListener : ISubscriberListener
    {
        private readonly IPubSubInfraConfig pubSubInfraConfig;
        private readonly IPubSubTopology topology;
        private readonly IEventHandlerService eventHandlerService;
        private readonly ILogger<SubscriberListener> logger;

        public SubscriberListener(
            IPubSubInfraConfig pubSubInfraConfig,
            IPubSubTopology topology,
            IEventHandlerService eventHandlerService,
            ILogger<SubscriberListener> logger)
        {
            this.pubSubInfraConfig = pubSubInfraConfig;
            this.topology = topology;
            this.eventHandlerService = eventHandlerService;
            this.logger = logger;
        }

        public void StartListening()
        {
            foreach (var subscriber in this.topology.ReadOnlySubscribers)
            {
                this.Subscribe(
                    this.pubSubInfraConfig.ProjectId,
                    subscriber);
            }
        }

        private void Subscribe(string projectId, SubscriberInfo subscriberInfo)
        {
            var subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriberInfo.SubscriptionName);

            var subClientBuilder = new SubscriberClientBuilder
            {
                SubscriptionName = subscriptionName,
                EmulatorDetection = this.topology.IsEmulatorActivated
                    ? EmulatorDetection.EmulatorOrProduction
                    : EmulatorDetection.ProductionOnly
            };

            try
            {
                var subClient = subClientBuilder.Build();

                subClient.StartAsync(async (message, _) =>
                {
                    try
                    {
                        await this.eventHandlerService.HandleEventAsync(this.topology, subscriberInfo, message);

                        // accnowladge the message
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {
                        // if any bigger exeptions occur try to reprocess the
                        // message by returning not-accnowladged and it will retry calling it
                        this.logger.Log(LogLevel.Error, ex.Message);

                        return SubscriberClient.Reply.Nack;
                    }
                });

                this.logger.Log(LogLevel.Information, $"Successfully subscribed to {subscriberInfo.SubscriptionName} in project {projectId}");
            }
            catch (Exception)
            {
                this.logger.Log(LogLevel.Debug, $"Something went wrong while subscribing to {subscriberInfo.SubscriptionName} in project {projectId}");
            }
        }
    }
}
