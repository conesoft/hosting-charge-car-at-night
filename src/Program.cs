using EvState.HttpClients;
using Microsoft.AspNetCore.Http.Extensions;

var configuration = new ConfigurationBuilder().AddJsonFile(Conesoft.Hosting.Host.GlobalSettings.Path).Build();
var client = new ECarUpHttpClient(new HttpClient(), configuration);

var runAt = TimeSpan.FromHours(22);
var runFor = TimeSpan.FromHours(5);


while (true)
{
    var start = (int)Math.Floor(runAt.TotalMilliseconds - (DateTime.Now - DateTime.Today).TotalMilliseconds);
    var end = (int)Math.Floor((runAt + runFor).TotalMilliseconds - (DateTime.Now - DateTime.Today).TotalMilliseconds);

    if (start <= 0 && end > 0)
    {
        await Run();
    }

    await Task.Delay((start + 86400000) % 86400000);
}

async Task Run()
{
    if ((await client.State()).Length == 0)
    {
        await client.StartCharging(runFor);

        var message = new
        {
            title = $"Charging started",
            message = $"Charging at Plattenstrasse 5 in Burg started",
            image_url = $"https://i.imgur.com/OrOZqXt.jpg",
            type = "Server"
        };

        await new HttpClient().GetAsync($@"https://wirepusher.com/send?id=mpgpt&title={message.title}&message={message.message}&type={message.type}&image_url={message.image_url}");
    }
    else
    {
        var message = new
        {
            title = $"Charging in progress",
            message = $"Charging at Plattenstrasse 5 in Burg is in progress",
            image_url = $"https://i.imgur.com/xP60kt7.png",
            type = "Server"
        };

        await new HttpClient().GetAsync($@"https://wirepusher.com/send?id=mpgpt&title={message.title}&message={message.message}&type={message.type}&image_url={message.image_url}");
    }
};