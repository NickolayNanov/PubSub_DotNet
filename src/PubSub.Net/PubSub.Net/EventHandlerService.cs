using Google.Cloud.PubSub.V1;
using PubSub.Net.Enums;
using PubSub.Net.Interfaces;
using System.Text.Json;

namespace PubSub.Net
{
    public class EventHandlerService : IEventHandlerService
    {
        public async Task HandleEventAsync(IPubSubTopology topology, SubscriberInfo subscriberInfo, PubsubMessage message)
        {
            var scopedSp = topology.ServiceProvider;

            // gets the generic T argument passed to ISubscriber<T>
            var messageType = subscriberInfo
                .SubscriberTypeInfo
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>))!
                .GetGenericArguments()[0];

            if (messageType.Name != subscriberInfo.MessageType.Name)
            {
                var errorMessage = $"The subscriber {subscriberInfo.SubscriberTypeInfo.Name} has different message type from {subscriberInfo.MessageType.Name}";
                throw new InvalidOperationException(errorMessage);
            }

            // make generic PubSubContext<T> type with the messageType type
            var contextType = typeof(PubSubContext<>).MakeGenericType(messageType);
            var parsedMessage = JsonSerializer.Deserialize(message.Data.ToStringUtf8(), messageType);

            // make generic PubSubPipeline<T> type with the messageType type
            var pipelineType = typeof(PubSubPipeline<>).MakeGenericType(messageType);

            // stores the use method in a MethodInfo variable so we can call it later
            // stores the invokeAsync method, with generic with the messageType in a MethodInfo
            // variable so we can call it later
            var useMethod = pipelineType.GetMethod(nameof(PubSubPipeline<IEventBase>.Use));
            var invokeAsyncMethod = typeof(IPipelineMiddleware)
                                    .GetMethod(nameof(IPipelineMiddleware.InvokeAsync))
                                    .MakeGenericMethod(messageType);

            // gets the pre/post middlewares
            var (preMiddlewares, postMiddlewares) = this.AquireMiddlewares(topology);

            var pipelinePreInstance = Activator.CreateInstance(pipelineType);

            // creates the lambda delegate required as an argument for the
            // InvokeAsync method (stored in a variable)
            void CallUseMethod(
                object pipelineInstance,
                IPipelineMiddleware middleware)
            {
                var delegateType = typeof(Func<,,>).MakeGenericType(contextType, typeof(Func<Task>), typeof(Task));
                var middlewareDelegate = Delegate.CreateDelegate(delegateType, middleware, invokeAsyncMethod);

                useMethod.Invoke(pipelineInstance, new object[] { middlewareDelegate });
            }

            // call the Use(...) method for each of the pre middlewares
            preMiddlewares.ForEach(middleware =>
            {
                CallUseMethod(pipelinePreInstance, middleware);
            });

            var pipelinePostInstance = Activator.CreateInstance(pipelineType);

            // call the Use(...) method for each of the post middlewares
            postMiddlewares.ForEach(middleware =>
            {
                CallUseMethod(pipelinePostInstance, middleware);
            });

            // makes generic PubSubPipeline<T> type with the messageType
            var genericPubSubPipelinType = typeof(PubSubPipeline<>).MakeGenericType(messageType);

            var context = Activator.CreateInstance(contextType, new object[] { parsedMessage, subscriberInfo.SubscriptionName });

            // stores the PubSubPipeline<T>.ExecuteAsync method so we can call it
            const string executeAsyncMethodName = nameof(PubSubPipeline<IEventBase>.ExecuteAsync);
            var executeAsyncMethod = genericPubSubPipelinType.GetMethod(executeAsyncMethodName);

            // invokes the executeAsyncMethod with the context so it can be modified
            // as needed via the middlewares
            await (Task)executeAsyncMethod.Invoke(pipelinePreInstance, new object[] { context });

            // creates an instance of the target subscriber via the DI in order
            // to support DI in the subsriber
            var subscriberInstance = scopedSp.GetService(subscriberInfo.SubscriberTypeInfo);

            if (subscriberInstance == null)
            {
                var errorMessage = $"A subscriber of type {subscriberInfo.SubscriberTypeInfo.Name} could not be found in the registered services. Please check the DI.";
                throw new InvalidOperationException(errorMessage);
            }

            try
            {
                // try to call the HandleEventAsync(MethodInfo) stored in
                // the subInfo using the subscriber instance created via the DI(serviceProvider)
                await (Task)subscriberInfo!
                .HandleEventAsync
                .Value
                .Invoke(subscriberInstance, new object[] { context });
            }
            catch (Exception)
            {
                // if any exceptions occur handle them and call the
                // HandleFailureEventAsync(MethodInfo) stored in the subInfo using
                // the subscriber instance created via the DI(serviceProvider)
                // WITH THE SAME CONTEXT from the initial request
                subscriberInfo!
                .HandleFailureEventAsync
                .Value
                .Invoke(subscriberInstance, new object[] { context });
            }

            // invokes again the executeAsyncMethod method with
            // the pipelinePostInstance and THE SAME CONTEXT
            await (Task)executeAsyncMethod.Invoke(pipelinePostInstance, new object[] { context });
        }

        private (List<IPipelineMiddleware> Pre, List<IPipelineMiddleware> Post) AquireMiddlewares(IPubSubTopology topology)
        {
            var defaultProcesses = topology
                .ReadOnlyDefaultProcesses
                .Where(p => p.PipelineType == PipelineType.SubPipeline)
                .ToList();

            var customPreProcesses = defaultProcesses.Where(p => p.PrePost == PrePost.Pre).ToList();
            var customPostProcesses = defaultProcesses.Where(p => p.PrePost == PrePost.Post).ToList();

            return (customPreProcesses, customPostProcesses);
        }
    }
}
