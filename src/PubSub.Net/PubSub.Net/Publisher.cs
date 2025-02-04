using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using PubSub.Net.Interfaces;
using System.Text.Json;

namespace PubSub.Net
{
    public class Publisher : IPublisher
    {
        private readonly IPubSubInfraConfig pubSubInfraConfig;
        private readonly ILogger<Publisher> logger;

        public Publisher(
            IPubSubInfraConfig pubSubInfraConfig,
            ILogger<Publisher> logger)
        {
            this.pubSubInfraConfig = pubSubInfraConfig;
            this.logger = logger;
        }

        public async Task PublishMessageAsync<TEvent>(string topicId, TEvent @event)
            where TEvent : IEventBase
        {
            var projectId = this.pubSubInfraConfig.ProjectId;
            var topicName = new TopicName(projectId, topicId);
            @event.Id = Guid.NewGuid();

            var publisher = await new PublisherServiceApiClientBuilder()
            {
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.BuildAsync();

            try
            {
                var jsonData = JsonSerializer.Serialize(@event);

                var pubsubMessage = new PubsubMessage
                {
                    MessageId = @event.Id.ToString(),
                    Data = ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes(jsonData)),
                    PublishTime = DateTime.UtcNow.ToTimestamp(),
                    /*Attributes = ...*/
                };

                var publishResponse = await publisher.PublishAsync(topicName, new[] { pubsubMessage });

                this.logger.LogInformation($"Published message with id: {publishResponse.MessageIds.First()} and type: {@event.GetType().Name}");
            }
            catch (Exception ex)
            {
                // TODO: mby extend this
                this.logger.LogError($"Error publishing message: {ex.Message}");
                throw;
            }
        }
    }
}
