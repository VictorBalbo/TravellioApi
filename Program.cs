using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Database connections
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConnectionString));

// Add Dependency Injection
builder.Services.AddScoped<ITripRepository, TripRepository>();

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();