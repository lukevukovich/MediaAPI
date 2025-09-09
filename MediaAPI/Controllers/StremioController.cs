using Microsoft.AspNetCore.Mvc;
using MediaAPI.Services;
using MediaAPI.Models.Stremio;

namespace MediaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StremioController : ControllerBase
    {
        private readonly IStremioService _stremioService;

        public StremioController(IStremioService stremioService)
        {
            _stremioService = stremioService;
        }

        /// <summary>
        /// Get the Stremio add-on manifest for Luke's Catalogs.
        /// </summary>
        [HttpGet("manifest.json")]
        public ActionResult<Manifest> GetManifest()
        {
            var manifest = new Manifest
            {
                Id = "stremio.luke.catalog",
                Version = "1.0.1",
                Name = "Luke's Catalogs",
                Description = "A collection of Luke's favorite catalogs.",
                Logo = "http://media-api.us-east-2.elasticbeanstalk.com/images/logo.png",
                Types = ["movie", "series"],
                Resources = ["catalog"],
                IdPrefixes = ["tt"],
                Catalogs = [
                    new Catalog { Type = "movie", Id = "slasher", Name = "Luke's Slasher Films", Extra = [
                        new Extra {
                            Name = "franchise",
                            Options = ["Halloween", "Friday the 13th", "A Nightmare on Elm Street", "Texas Chainsaw Massacre", "Scream", "Child's Play", "Terrifier", "Hatchet", "Thanksgiving", "Black Christmas"],
                            IsRequired = true
                        }
                    ]},
                    new Catalog { Type = "movie", Id = "horror", Name = "Luke's Horror Movies", Extra = [
                        new Extra {
                            Name = "franchise",
                            Options = ["The Conjuring Universe", "Insidious", "Paranormal Activity", "The Exorcist", "Saw", "Pet Sematary", "The Grudge", "The Evil Dead", "Ari Aster"],
                            IsRequired = true
                        }
                    ]},
                    new Catalog { Type = "movie", Id = "gangster", Name = "Luke's Gangster Movies" }
                ]
            };

            return Ok(manifest);
        }

        /// <summary>
        /// Get the Slasher Films catalog from MDBList, optionally filtered by franchise.
        /// </summary>
        [HttpGet("catalog/movie/slasher/{franchise}.json")]
        public async Task<IActionResult> GetSlasherCatalogAsync([FromRoute] string franchise, CancellationToken cancellationToken = default)
        {
            var owner = "slander2328";
            var name = "slasher-movies";
            var franchiseMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "halloween", ["halloween", "myers"] },
                { "friday the 13th", ["friday the 13th", "jason", "voorhees", "never hike alone"] },
                { "a nightmare on elm street", ["nightmare on elm street", "new nightmare", "freddy"] },
                { "texas chainsaw massacre", ["texas chainsaw", "texas chain saw", "leatherface"] },
                { "scream", ["scream", "woodsboro"] },
                { "child's play", ["child's play", "chucky"] },
                { "terrifier", ["terrifier", "art the clown", "the mean one"] },
                { "hatchet", ["hatchet", "victor crowley"] },
                { "thanksgiving", ["thanksgiving"] },
                { "black christmas", ["black christmas"] }
            };

            var response = await _stremioService.ProxyCatalogMetasAsync(owner, name, franchise, franchiseMap, CatalogSortEnum.YearAscending, cancellationToken);
            if (!response.Success)
                return Problem(response.ErrorMessage, statusCode: response.StatusCode);
            
            return Ok(response.Value);
        }

        /// <summary>
        /// Get the Horror Movies catalog from MDBList, optionally filtered by franchise.
        /// </summary>
        [HttpGet("catalog/movie/horror/{franchise}.json")]
        public async Task<IActionResult> GetHorrorCatalogAsync([FromRoute] string franchise, CancellationToken cancellationToken = default)
        {
            var owner = "chimaklesel";
            var name = "horror";
            var franchiseMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "the conjuring universe", ["the conjuring", "annabelle", "the nun", "the curse of la llorona"] },
                { "insidious", ["insidious"] },
                { "paranormal activity", ["paranormal activity"] },
                { "the exorcist", ["exorcist"] },
                { "saw", ["saw"] },
                { "pet sematary", ["pet sematary"] },
                { "the grudge", ["grudge"] },
                { "the evil dead", ["evil dead", "army of darkness"] },
                { "ari aster", ["hereditary", "midsommar"] }
            };

            var response = await _stremioService.ProxyCatalogMetasAsync(owner, name, franchise, franchiseMap, CatalogSortEnum.YearAscending, cancellationToken);
            if (!response.Success)
                return Problem(response.ErrorMessage, statusCode: response.StatusCode);

            return Ok(response.Value);
        }

        /// <summary>
        /// Get the Gangster Movies catalog from MDBList.
        /// </summary>
        [HttpGet("catalog/movie/gangster.json")]
        public async Task<IActionResult> GetGangsterCatalogAsync(CancellationToken cancellationToken = default)
        {
            var owner = "slander2328";
            var name = "gangster-movies-xhebzvlfgb";

            var response = await _stremioService.ProxyCatalogMetasAsync(owner, name, null, null, null, cancellationToken);
            if (!response.Success)
                return Problem(response.ErrorMessage, statusCode: response.StatusCode);

            return Ok(response.Value);
        }
    }
}