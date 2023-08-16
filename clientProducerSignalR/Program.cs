using System.CommandLine;
using System.Text.Json;

namespace clientProducerSignalR;
public class Program
{
    public static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Lado producer do teste de carga do SignalR");
        var urlArgument = new Argument<string>(
            "url",
            "URL do servidor SignalR");

        var durationOption = new Option<int>(
            "--duration",
            () => int.MaxValue,
            "Duração do teste em segundos");
        var clientsOption = new Option<int>(
            "--clients",
            () => 1,
            "Quantidade de clientes que serão simulados");
        var mpsOption = new Option<int>(
            "--mps",
            () => 100,
            "Numero de mensagens por segundo, de cada cliente");
        var consumerClientsOption = new Option<int>(
            "--consumerClients",
            () => 100,
            "Quantidade de clientes do lado consumidor.");
        var messageSizeOption = new Option<int>(
            "--messageSize",
            () => 1024,
            "Tamanho da mensagem a ser enviada");
        var commentOption = new Option<string>(
            "--comment",
            () => "",
            "Comentário que será adicionado ao arquivo de resultado");

        rootCommand.Add(urlArgument);
        rootCommand.Add(durationOption);
        rootCommand.Add(clientsOption);
        rootCommand.Add(mpsOption);
        rootCommand.Add(consumerClientsOption);
        rootCommand.Add(messageSizeOption);
        rootCommand.Add(commentOption);

        rootCommand.SetHandler(RunAsync,
            urlArgument, durationOption, clientsOption, mpsOption, consumerClientsOption, messageSizeOption, commentOption
        );

        await rootCommand.InvokeAsync(args);
    }

    private static async Task RunAsync(string url, int duration, int clients, int mps, int consumerClients, int messageSize, string comments)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var init = new Initializer(TimeSpan.FromSeconds(duration), clients, url, comments, mps, messageSize, consumerClients);
        var orchestrator = new ConnectionOrchestrator(init);
        DisplayImage(init);

        cancellationTokenSource.CancelAfter(init.Duration);
        try
        {
            await orchestrator.RunAsync(cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        { }

        Console.WriteLine("Terminating...");
        await orchestrator.CloseAllConnection();
    }

    //Function that will display a console that represent a rocket
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
        Console.WriteLine("Aplicação: Produtora");
        Console.WriteLine($"Parametros: {JsonSerializer.Serialize(init, new JsonSerializerOptions { WriteIndented = true })}");
        Console.WriteLine("------------------------------------\n");
    }
}