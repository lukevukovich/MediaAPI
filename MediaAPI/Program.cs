using MediaAPI.Options;
using Microsoft.Extensions.Options;
using MediaAPI.Http;
using MediaAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MdbListOptions>(builder.Configuration.GetSection("MdbListOptions"));
builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("TmdbOptions"));

builder.Services.AddHttpClient<IMdbListClient, MdbListClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<MdbListOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddHttpClient<ITmdbClient, TmdbClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<TmdbOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddScoped<ITmdbService, TmdbService>();
builder.Services.AddScoped<IMdbListService, MdbListService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();