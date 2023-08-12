namespace sharedCore;

public class InitializerBase
{
    public TimeSpan Duration { get; set; }
    public int Clients { get; set; }
    public string WithUrl { get; set; }

    public string Path { get; set; }

    public string Comments { get; set; }

    public InitializerBase(string[] args)
    {
        var indexTime = Array.IndexOf(args, "--duration");

        if (indexTime == -1) Duration = TimeSpan.FromMilliseconds(double.MaxValue);
        else Duration = TimeSpan.FromSeconds(double.Parse(args[indexTime + 1]));

        var clientsIndex = Array.IndexOf(args, "--clients");

        if (clientsIndex == -1) Clients = 1;
        else Clients = int.Parse(args[clientsIndex + 1]);

        var withUrlIndex = Array.IndexOf(args, "--url");

        if (withUrlIndex == -1) throw new ArgumentException("Type is required");
        else WithUrl = args[withUrlIndex + 1];

        var path = Array.IndexOf(args, "--path");
        if (path == -1) throw new ArgumentException("Path is required");
        else Path = args[path + 1];


        var comment = Array.IndexOf(args, "--comment");
        if (comment == -1) Comments = string.Empty;
        else Comments = args[comment + 1];
    }

}