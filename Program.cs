using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

app.MapGet("/", async (YabiDb db) => await db.Yabis.OrderBy(y => y.DateTime).LastAsync());

app.MapGet("/yabi", async (IHttpClientFactory clientFactory, YabiDb db) =>
{
    var Yabi = await YabiIndex.Build(clientFactory);

    await db.AddAsync(Yabi);
    await db.SaveChangesAsync();

    return Results.Created("OK", Yabi);
}); 

app.Run();