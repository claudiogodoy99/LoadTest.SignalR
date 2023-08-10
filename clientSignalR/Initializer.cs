using sharedCore;

namespace clientSignalR;

public class Initializer : InitializerBase
{
    public bool Reconnect { get; set; }
    public Initializer(string[] args) : base(args)
    {
        var reconectIndex = Array.IndexOf(args, "--reconnect");

        if (reconectIndex == -1) Reconnect = false;
        else Reconnect = true;
    }
}