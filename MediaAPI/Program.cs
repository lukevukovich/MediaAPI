using MdbListApi.Options;
using Microsoft.Extensions.Options;
using MdbListApi.Http;
using MdbListApi.Services;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/list/{owner}/{name}", async (string owner, string name, IMdbListService service, CancellationToken ct) => await service.ProxyListAsync(owner, name, ct))
    .WithName("GetList")
    .WithOpenApi(op =>
    {
        op.Summary = "Obtain a list from MDBList with TMDB poster URLs.";
        op.Description = "Fetches a list from MDBList and enriches it with poster URLs from TMDB. Uses IMDB IDs to fetch poster paths.";
        return op;
    });

app.MapGet("/poster/{imdb_id}", async (string imdb_id, ITmdbService service, CancellationToken ct) => await service.ProxyPosterPathAsync(imdb_id, ct))
    .WithName("GetPoster")
    .WithOpenApi(op =>
    {
        op.Summary = "Obtain the TMDB poster path for a given IMDB ID.";
        op.Description = "Fetches the poster path from TMDB using the provided IMDB ID.";
        return op;
    });

app.Run();