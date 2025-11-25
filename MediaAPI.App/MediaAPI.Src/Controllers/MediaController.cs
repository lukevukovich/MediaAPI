using Microsoft.AspNetCore.Mvc;
using MediaAPI.Services;

namespace MediaAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMdbListService _mdbListService;
    private readonly ITmdbService _tmdbService;

    public MediaController(IMdbListService mdbListService, ITmdbService tmdbService)
    {
        _mdbListService = mdbListService;
        _tmdbService = tmdbService;
    }

    /// <summary>
    /// Obtain a list from MDBList with TMDB poster URLs.
    /// </summary>
    /// <param name="owner">Owner of the MDBList</param>
    /// <param name="name">Name of the MDBList</param>
    /// <param name="cancellationToken">Token used to cancel the request</param>
    [HttpGet("list/{owner}/{name}")]
    public async Task<IActionResult> GetMdbListAsync(string owner, string name, CancellationToken cancellationToken = default)
    {
        var result = await _mdbListService.ProxyListAsync(owner, name, cancellationToken: cancellationToken);
        if (!result.Success)
        {
            return Problem(result.ErrorMessage, statusCode: result.StatusCode);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtain TMDB details for a given IMDB ID.
    /// </summary>
    /// <param name="imdb_id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("details/{imdb_id}")]
    public async Task<IActionResult> GetTmdbDetailsAsync(string imdb_id, CancellationToken cancellationToken = default)
    {
        var result = await _tmdbService.ProxyDetailsAsync(imdb_id, cancellationToken);
        if (!result.Success)
        {
            return Problem(result.ErrorMessage, statusCode: result.StatusCode);
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtain a TMDB poster URL for a given IMDB ID.
    /// </summary>
    /// <param name="imdb_id">The IMDB ID of the media item</param>
    /// <param name="cancellationToken">Token used to cancel the request</param>
    [HttpGet("poster/{imdb_id}")]
    public async Task<IActionResult> GetTmdbPosterAsync(string imdb_id, CancellationToken cancellationToken = default)
    {
        var result = await _tmdbService.ProxyPosterPathAsync(imdb_id, cancellationToken);
        if (!result.Success)
        {
            return Problem(result.ErrorMessage, statusCode: result.StatusCode);
        }
        return Ok(result.Value);
    }
}