using clientSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        DisplayImage();

        var cancellationToken = new CancellationTokenSource();

        var init = new Initializer(args);

        DateTime startTime = DateTime.Now;

        var messageAnalytics = new MessageAnalyticsBase(cancellationToken.Token);
        var orchestrator = new ConnectionOrchestrator(init, cancellationToken, messageAnalytics);

        while (cancellationToken.IsCancellationRequested == false)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (DateTime.Now - startTime > init.Duration)
            {
                Console.WriteLine("Terminating...");
                await orchestrator.CloseAllConnection();
                cancellationToken.Cancel();
            }
        }

        Console.WriteLine("Generating Results File...");
        var csvWriter = new CsvWriter(messageAnalytics, init);
        await csvWriter.RegisterTest();
        Console.WriteLine("Done!");
    }

    public static void DisplayImage()
    {
        // Rocket image with colors and background
        string rocketImage = @"
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
  ";

        // Define the information text
        Console.WriteLine(rocketImage);
        Console.WriteLine("------------------------------------");
        Console.WriteLine("Aplicação: Consumidora");
        Console.WriteLine("------------------------------------\n");

    }
}
