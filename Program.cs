using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Yabi.Models;
using Yabi.Cron;


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
builder.Services.AddHostedService<Scheduler>();


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

// Index generation recurrency

// The system should calculate once a day the index, and return it on the "/" route.
// The system could have a description of the calc to reach the index.

app.MapGet("/", async (YabiDb db) => await db.Yabis.OrderBy(y => y.DateTime).LastAsync());


app.Run();