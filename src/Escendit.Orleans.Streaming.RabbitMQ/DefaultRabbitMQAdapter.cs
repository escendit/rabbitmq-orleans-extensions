using Orleans.Runtime;
using Orleans.Streams;

namespace Escendit.Orleans.Streaming.RabbitMQ;

public class DefaultRabbitMQAdapter : IQueueAdapter
{
    public Task QueueMessageBatchAsync<T>(StreamId streamId, IEnumerable<T> events, StreamSequenceToken token,
        Dictionary<string, object> requestContext)
    {
        throw new NotImplementedException();
    }

    public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
    {
        throw new NotImplementedException();
    }

    public string Name { get; } = string.Empty;
    public bool IsRewindable { get; } = false;
    public StreamProviderDirection Direction { get; } = StreamProviderDirection.ReadWrite;
}