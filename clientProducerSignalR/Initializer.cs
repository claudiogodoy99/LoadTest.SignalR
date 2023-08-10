using sharedCore;

namespace clientProducerSignalR;

public class Initializer : InitializerBase
{
    public int Mps { get; set; }
    public int StartingId {get;set;}

    public Initializer(string[] args) : base(args)
    {
        var mpsIndex = Array.IndexOf(args, "--mps");

        if (mpsIndex == -1) Mps = 100;
        else Mps = int.Parse(args[mpsIndex + 1]);

        var startingId = Array.IndexOf(args, "--startingDataId");

        if(startingId == -1) StartingId = 0;
        else StartingId = int.Parse(args[startingId + 1]);

    }

}

