using sharedCore;

namespace clientSignalR;

public class Initializer : InitializerBase
{
    public bool Reconnect { get; init; }
    public string Path { get; set; }

    public Initializer(string[] args) : base(args)
    {
        var path = Array.IndexOf(args, "--path");
        if (path == -1) throw new ArgumentException("Path is required");
        else Path = args[path + 1];

        var reconnect = Array.IndexOf(args, "--reconnect");
        if (reconnect == -1) Reconnect = false;
        else Reconnect = true;
    }
}