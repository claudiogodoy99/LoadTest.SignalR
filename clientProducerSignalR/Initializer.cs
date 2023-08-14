using sharedCore;

namespace clientProducerSignalR;

public class Initializer : InitializerBase
{
    public int Mps { get; init; }
    public int MessageSize { get; init; }
    public int ConsumerClients { get; init; }
}

