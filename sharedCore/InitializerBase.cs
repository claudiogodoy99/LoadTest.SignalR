namespace sharedCore;

public abstract class InitializerBase
{
    public InitializerBase(TimeSpan duration, int clients, string withUrl, string comments)
    {
        Duration = duration;
        Clients = clients;
        WithUrl = withUrl;
        Comments = comments;
    }
    public TimeSpan Duration { get; }
    public int Clients { get; }
    public string WithUrl { get; }
    public string Comments { get; }

}