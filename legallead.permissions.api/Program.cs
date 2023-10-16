using legallead.permissions.api;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var environmentName = builder.Environment.EnvironmentName;
var config =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environmentName}.json", true)
        .AddEnvironmentVariables()
        .Build();

builder.Services.AddControllers();
builder.Services.AddSingleton<IConfiguration>(config);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
