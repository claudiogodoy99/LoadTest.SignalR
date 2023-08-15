using clientSignalR;
using sharedCore;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var cancellationToken = new CancellationTokenSource();
        var messageAnalytics = new MessageAnalyticsBase(cancellationToken.Token);

        var init = new Initializer(args);

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
