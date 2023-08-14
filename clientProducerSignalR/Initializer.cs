using sharedCore;

namespace clientProducerSignalR;

public class Initializer : InitializerBase
{
    public int Mps { get; set; }
    public int StartingId { get; set; }
    public int MessageSize { get; set; }
    public int Delay { get; set; }

    public Initializer(string[] args) : base(args)
    {
        var mpsIndex = Array.IndexOf(args, "--mps");

        if (mpsIndex == -1) Mps = 100;
        else Mps = int.Parse(args[mpsIndex + 1]);

        var startingId = Array.IndexOf(args, "--startingDataId");

        if (startingId == -1) StartingId = 0;
        else StartingId = int.Parse(args[startingId + 1]);

        var messageSize = Array.IndexOf(args, "--messageSize");

        if (messageSize == -1) MessageSize = 1000;
        else MessageSize = int.Parse(args[MessageSize + 1]);

        var delay = Array.IndexOf(args, "--delay");

        if (delay == -1) Delay = 1000;
        else Delay = int.Parse(args[Delay + 1]);

    }

}

