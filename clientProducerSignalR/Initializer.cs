using sharedCore;

namespace clientProducerSignalR;

public class Initializer : InitializerBase
{
    public int Mps { get; init; }
    public int MessageSize { get; init; }
    public int ConsumerClients { get; init; }

    public Initializer(string[] args) : base(args)
    {
        var mps = Array.IndexOf(args, "--mps");
        if(mps == -1 ) Mps = 100;
        else Mps = int.Parse(args[mps+1]);

        var messageSize = Array.IndexOf(args, "--messageSize");
        if(messageSize == -1 ) MessageSize = 1024;
        else MessageSize = int.Parse(args[messageSize+1]);

        var consumerClients = Array.IndexOf(args, "--consumerClients");
        if(consumerClients == -1 ) ConsumerClients = 1;
        else ConsumerClients = int.Parse(args[consumerClients+1]);
    }

    
}

