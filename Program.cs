using Microsoft.OpenApi.Models;
using System.Text.Json;
using Yabi;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "YABI API",
        Description = "Yet another bitcoin index",
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

app.MapGet("/", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    var response = await client.GetAsync("https://api.alternative.me/fng/");

    if (response.IsSuccessStatusCode)
    {
        var SerializerOptions = new JsonSerializerOptions() { };
        var json = await response.Content.ReadAsStringAsync();

        FearAndGreedIndex? fearAndGreedIndex = JsonSerializer.Deserialize<FearAndGreedIndex>(json);

        Console.WriteLine(json);
        Console.WriteLine(fearAndGreedIndex?.Data?[0].Value);

        return Results.Ok(fearAndGreedIndex); 
   
    }
    else
    {
        return Results.StatusCode((int)response.StatusCode);
    }
});

app.Run();