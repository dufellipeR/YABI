using Microsoft.OpenApi.Models;
using System.Text.Json;
using Yabi.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

var builder = WebApplication.CreateBuilder(args);


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

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapGet("/fng", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    var response = await client.GetAsync("https://api.alternative.me/fng/");

    if (response.IsSuccessStatusCode)
    {
        var SerializerOptions = new JsonSerializerOptions() { };
        var json = await response.Content.ReadAsStringAsync();

        FearAndGreedIndex? fearAndGreedIndex = JsonSerializer.Deserialize<FearAndGreedIndex>(json);

        Console.WriteLine(fearAndGreedIndex?.Data?[0].Value);

        return Results.Ok(fearAndGreedIndex); 
   
    }
    else
    {
        return Results.StatusCode((int)response.StatusCode);
    }
});


app.MapGet("/mayer", async (IHttpClientFactory clientFactory) =>
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
        // Load the website
        driver.Navigate().GoToUrl(url);

        // Wait until the information is loaded
        var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(3));
        var element = wait.Until(d => d.FindElement(By.Id("mayer-multiple")));
        wait.Until(d => element.Text != "Loading...");

        // Scrape the information
        mayer = element.Text;
        Console.WriteLine(mayer);
    }
    finally
    {
        // Close the browser
        driver.Quit();
    }

    return Results.Ok(mayer);
});

app.MapGet("/rsi", async (IHttpClientFactory clientFactory) =>
{

    return Results.Ok();
});

app.Run();