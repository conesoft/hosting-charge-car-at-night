using EvState.HttpClients;

var configuration = new ConfigurationBuilder().AddJsonFile(Conesoft.Hosting.Host.GlobalSettings.Path).Build();
var client = new ECarUpHttpClient(new HttpClient(), configuration);
var wirepusher = configuration["wirepusher:url"];

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
        await Notify("Charging started", "Charging at Plattenstrasse 5 in Burg started", "https://i.imgur.com/xP60kt7.png", "Server");
    }
    else
    {
        await Notify("Charging in progress", "Charging at Plattenstrasse 5 in Burg in progress", "https://i.imgur.com/xP60kt7.png", "Server");
    }
};

Task Notify(string title, string message, string imageUrl, string type) => new HttpClient().GetAsync(wirepusher + $@"title={title}&message={message}&type={type}&image_url={imageUrl}");