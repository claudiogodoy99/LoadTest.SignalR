using sharedCore;

namespace clientSignalR;

public sealed class Initializer : InitializerBase
{
    public Initializer(TimeSpan duration, int clients, string withUrl, string comments, string path, bool reconnect)
        : base(duration, clients, withUrl, comments)
    {
        Path = path;
        Reconnect = reconnect;
    }

    public string Path { get; init; }
    public bool Reconnect { get; init; }
}