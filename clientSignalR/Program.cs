using clientSignalR;
using sharedCore;
using System.CommandLine;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Lado consumer do teste de carga do SignalR");
        var urlArgument = new Argument<string>(
            "url",
            "URL do servidor SignalR");
        var pathArgument = new Argument<string>(
            "path",
            "Caminho onde será salvo o arquivo de resultado");

        var reconnectOption = new Option<bool>(
            "--reconnect",
            () => false,
            "Caso habilitado força a aplicação a recriar os clients a cada 10 segundos");
        var durationOption = new Option<int>(
            "--duration",
            () => int.MaxValue,
            "Duração do teste em segundos");
        var clientsOption = new Option<int>(
            "--clients",
            () => 1,
            "Quantidade de clientes que serão simulados");
        var commentOption = new Option<string>(
            "--comment",
            () => "",
            "Comentário que será adicionado ao arquivo de resultado");

        rootCommand.Add(urlArgument);
        rootCommand.Add(pathArgument);
        rootCommand.Add(reconnectOption);
        rootCommand.Add(durationOption);
        rootCommand.Add(clientsOption);
        rootCommand.Add(commentOption);

        rootCommand.SetHandler(RunAsync,
            urlArgument, pathArgument, reconnectOption, durationOption, clientsOption, commentOption
        );

        await rootCommand.InvokeAsync(args);
    }
    private static async Task RunAsync(string url, string path, bool reconnect, int duration, int clients, string comments)
    {
        var cancellationToken = new CancellationTokenSource();
        var messageAnalytics = new MessageAnalyticsBase(cancellationToken.Token);

        var init = new Initializer
        {
            Reconnect = reconnect,
            Duration = TimeSpan.FromSeconds(duration),
            Clients = clients,
            Comments = comments,
            WithUrl = url,
            Path = path
        };
        var orchestrator = new ConnectionOrchestrator(init, cancellationToken, messageAnalytics);

        DisplayImage(init);
        cancellationToken.CancelAfter(init.Duration);
        try
        {
            await orchestrator.StartAsync();
        }
        catch (OperationCanceledException)
        {
        }

        Console.WriteLine("Terminating...");
        await orchestrator.CloseAllConnection();

        Console.WriteLine("Generating Results File...");
        var csvWriter = new CsvWriter(messageAnalytics, init);
        await csvWriter.RegisterTest();
        Console.WriteLine("Done!");
    }

    public static void DisplayImage(Initializer init)
    {
        // Rocket image with colors and background
        #region string rocketImage = "..."
        string rocketImage = """
            ___________             __                                 
            \__    _______   ______/  |_  ____                         
              |    |_/ __ \ /  ___\   ___/ __ \                        
              |    |\  ___/ \___ \ |  | \  ___/                        
              |____| \___  /____  >|__|  \___  >                       
                         \/     \/           \/                        
                .___                                                   
              __| _/____                                               
             / __ _/ __ \                                              
            / /_/ \  ___/                                              
            \____ |\___  >                                             
                 \/    \/                                              

              ____ _____ _______  _________                            
            _/ ___\\__  \\_  __ \/ ___\__  \                           
            \  \___ / __ \|  | \/ /_/  / __ \_                         
             \___  (____  |__|  \___  (____  /                         
                 \/     \/     /_____/     \/                          

              ____  ____   _____                                       
            _/ ___\/  _ \ /     \                                      
            \  \__(  <_> |  Y Y  \                                     
             \___  \____/|__|_|  /                                     
                 \/            \/                                      
              ________.__                     .__           __________ 
             /   _____|__| ____   ____ _____  |  |          \______   \
             \_____  \|  |/ ___\ /    \\__  \ |  |    ______ |       _/
             /        |  / /_/  |   |  \/ __ \|  |__ /_____/ |    |   \
            /_______  |__\___  /|___|  (____  |____/         |____|_  /
                    \/  /_____/      \/     \/                      \/
            """;
        #endregion
        // Define the information text
        Console.WriteLine(rocketImage);
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Aplicação: Consumidora");
        Console.WriteLine($"Parametros: {JsonSerializer.Serialize(init, new JsonSerializerOptions { WriteIndented = true })}");
        Console.WriteLine("------------------------------------\n");

    }
}
