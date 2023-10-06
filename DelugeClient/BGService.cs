namespace DelugeClient
{
    public class BGService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(Endpoints.client != null)
                {
                    try
                    {
                        Console.WriteLine("\nClient Login Attempt");
                        await Endpoints.client.LoginAsync(Endpoints.deluged_pass);
                        await Task.Delay(new TimeSpan(0, 30, 0), stoppingToken);
                    }
                    catch { }

                }
                await Task.Delay(1000);
            }
        }
    }
}
