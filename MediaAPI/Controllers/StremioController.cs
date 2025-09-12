using Microsoft.AspNetCore.Mvc;
using MediaAPI.Services;
using MediaAPI.Models.Stremio;
using System.Collections.Specialized;

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
                Version = "1.0.2",
                Name = "Luke's Catalogs",
                Description = "A collection of Luke's favorite catalogs.",
                Logo = "https://www.media-api.dev/images/logo.png",
                Types = ["movie", "series"],
                Resources = ["catalog"],
                IdPrefixes = ["tt"],
                Catalogs = [
                    new Catalog { Type = "movie", Id = "slasher", Name = "Luke's Slasher Films", Extra = [
                        new Extra {
                            Name = "genre",
                            Options = ["All", "Halloween", "Friday the 13th", "A Nightmare on Elm Street", "Texas Chainsaw Massacre", "Scream", "Child's Play", "Terrifier", "Hatchet", "Black Christmas", "Jeepers Creepers", "I Know What You Did Last Summer", "The Strangers", "Miscellaneous"],
                            IsRequired = true
                        }
                    ]},
                    new Catalog { Type = "movie", Id = "horror", Name = "Luke's Horror Movies", Extra = [
                        new Extra {
                            Name = "genre",
                            Options = ["All", "The Conjuring Universe", "Insidious", "Paranormal Activity", "The Exorcist", "Pet Sematary", "The Evil Dead", "The Grudge", "Alien", "Miscellaneous"],
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
        public async Task<IActionResult> GetSlasherCatalogFilterAsync([FromRoute] string franchise, CancellationToken cancellationToken = default)
        {
            var owner = "slander2328";
            var name = "slasher-movies";
            var franchiseMap = new OrderedDictionary(StringComparer.OrdinalIgnoreCase)
            {
                { "halloween", new List<string> { "halloween", "myers" } },
                { "friday the 13th", new List<string> { "friday the 13th", "jason", "voorhees", "never hike alone" } },
                { "a nightmare on elm street", new List<string> { "nightmare on elm street", "new nightmare", "freddy" } },
                { "texas chainsaw massacre", new List<string> { "texas chainsaw", "texas chain saw", "leatherface" } },
                { "scream", new List<string> { "scream", "woodsboro" } },
                { "child's play", new List<string> { "child's play", "chucky" } },
                { "terrifier", new List<string> { "terrifier", "art the clown" } },
                { "hatchet", new List<string> { "hatchet", "victor crowley" } },
                { "black christmas", new List<string> { "black christmas" } },
                { "jeepers creepers", new List<string> { "jeepers creepers", "the creeper" } },
                { "the strangers", new List<string> { "the strangers" } },
                { "miscellaneous", new List<string> { "american psycho", "thanksgiving", "the mean one", "heart eyes", "trick 'r treat", "my bloody valentine", "five nights at freddy's", "the shining", "candyman", "hellraiser", "wrong turn", "the hills have eyes", "children of the corn", "the terminator", "i know what you did last summer" } }
            };

            if (franchiseMap.Contains('='))
                franchise = franchise.Split('=')[1];

            var response = await _stremioService.ProxyCatalogMetasAsync(owner, name, franchise, franchiseMap, string.Equals(franchise, "miscellaneous", StringComparison.OrdinalIgnoreCase) ? CatalogSortEnum.NameAscending : CatalogSortEnum.YearAscending, cancellationToken);
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
            var franchiseMap = new OrderedDictionary(StringComparer.OrdinalIgnoreCase)
            {
                { "the conjuring universe", new List<string> { "the conjuring", "annabelle", "the nun", "the curse of la llorona" } },
                { "insidious", new List<string> { "insidious" } },
                { "paranormal activity", new List<string> { "paranormal activity" } },
                { "the exorcist", new List<string> { "exorcist" } },
                { "pet sematary", new List<string> { "pet sematary" } },
                { "the evil dead", new List<string> { "evil dead", "army of darkness" } },
                { "the grudge", new List<string> { "grudge" } },
                { "alien", new List<string> { "alien", "prometheus" } },
                { "miscellaneous", new List<string> { "hereditary", "midsommar", "longlegs", "it follows", "smile", "talk to me", "nosferatu", "nope", "get out", "the invisible man", "jaws", "cloverfield" } }
            };

            if (franchiseMap.Contains('='))
                franchise = franchise.Split('=')[1];

            var response = await _stremioService.ProxyCatalogMetasAsync(owner, name, franchise, franchiseMap, string.Equals(franchise, "miscellaneous", StringComparison.OrdinalIgnoreCase) ? CatalogSortEnum.NameAscending : CatalogSortEnum.YearAscending, cancellationToken);
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