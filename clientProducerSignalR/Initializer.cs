using sharedCore;

namespace clientProducerSignalR;

public sealed class Initializer : InitializerBase
{
    public Initializer(TimeSpan duration, int clients, string withUrl, string comments, int mps, int messageSize, int consumerClients) 
        : base(duration, clients, withUrl, comments)
    {
        Mps = mps;
        MessageSize = messageSize;
        ConsumerClients = consumerClients;
    }

    public int Mps { get; }
    public int MessageSize { get; }
    public int ConsumerClients { get; }
}

