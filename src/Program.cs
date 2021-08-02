using EvState.HttpClients;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Conesoft.Services.ChargeCarAtNight
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(Hosting.Host.GlobalSettings.Path).Build();
            var client = new ECarUpHttpClient(new HttpClient(), configuration);

            var lastRun = DateTime.MinValue;
            var runAt = TimeSpan.FromHours(22);
            var runFor = TimeSpan.FromHours(5);

            while (true)
            {
                await Task.Delay(1000 - DateTime.Now.Millisecond);

                Console.WriteLine($"tick @ {DateTime.Now}");

                if (lastRun < DateTime.Today && (DateTime.Now - DateTime.Today) > runAt)
                {
                    lastRun = DateTime.Today;

                    if((await client.State()).Length == 0)
                    {
                        await client.StartCharging(runFor);

                        Console.WriteLine($"started charging for {DateTime.Today}");
                    }
                }
            }
        }
    }
}
