using PubSub.Net.Interfaces;

namespace PubSub.Net
{
    public class PubSubConfiguration : IPubSubInfraConfig
    {
        public string ProjectId { get; set; }
    }
}
