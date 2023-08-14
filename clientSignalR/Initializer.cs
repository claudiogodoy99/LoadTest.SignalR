using sharedCore;

namespace clientSignalR;

public class Initializer : InitializerBase
{
    public string Path { get; init; }
    public bool Reconnect { get; init; }
}