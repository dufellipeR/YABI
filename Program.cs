using Microsoft.OpenApi.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Yabi.Models;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Yabi") ?? "Data Source=Yabi.db";


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "YABI API",
        Description = "Yet Another Bitcoin Index",
        Version = "v1"
    });
});
builder.Services.AddSqlite<YabiDb>(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "YABI API V1");
    });
}

app.UseHttpsRedirection();

// The system should calculate once a day the index, and return it on the "/" route.
// The system could have a description of the calc to reach the index.

/* 

# The score should be updated once a day

# Build a YABI INDEX like: 

# Fear & Greed index < 30 Add 5 to score
# Fear & Greed index < 20 Add 10 to score
# Fear & Greed index < 10 Add 15 to score

# Mayer Multiple Index < 2 Add 5 to score
# Mayer Multiple Index < 1 Add 10 to score
# Mayer Multiple Index < 0.50 Add 15 to score

# Relative strength index < 60 Add 5 to score
# Relative strength index < 50 Add 10 to score
# Relative strength index < 40 Add 15 to score

*/


app.MapGet("/mayer", () =>
{
    var url = "https://buybitcoinworldwide.com/mayer-multiple/";

    // Set up the browser
    var options = new ChromeOptions();
    options.AddArgument("--headless");
    options.AddArgument("--disable-gpu");
    options.AddArgument("--no-sandbox");
    options.AddArgument("--disable-dev-shm-usage");
    var driver = new ChromeDriver(options);

    var mayer = "";

    try
    {
        
        driver.Navigate().GoToUrl(url);

        // Wait until the information is loaded, damn SPA's
        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(3));
        var element = wait.Until(d => d.FindElement(By.Id("mayer-multiple")));
        wait.Until(d => element.Text != "Loading...");

        mayer = element.Text;
        Console.WriteLine(mayer);
    }
    finally
    {
        driver.Quit();
    }

    return Results.Ok(mayer);
});

app.MapGet("/yabi", async (IHttpClientFactory clientFactory) =>
{
    // Yabi max score
    const int YabiMax = 21;

    var Yabi = 0;

    var FngIndex = await FearAndGreedIndex.GetScore(clientFactory);

    var MayerIndex = await Mayer.GetScore();

    List<int> indexes = [FngIndex, MayerIndex];

    // Max sum  of indexes score
    var Max = indexes.Count * 99;

    var Sum = indexes.Sum();

    Yabi = YabiMax * Sum / Max;


    return Results.Ok(Yabi);
});

app.Run();