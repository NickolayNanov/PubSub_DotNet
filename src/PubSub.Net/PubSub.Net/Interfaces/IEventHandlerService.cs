using Google.Cloud.PubSub.V1;

namespace PubSub.Net.Interfaces
{
    public interface IEventHandlerService
    {
        Task HandleEventAsync(IPubSubTopology topology, SubscriberInfo subscriberInfo, PubsubMessage message);
    }
}
