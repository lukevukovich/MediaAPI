# MediaAPI
MediaAPI is a .NET Web API that aggregates media data from external sources like MDBList and TMDB.

## Media Endpoints
- `GET /media/list/{owner}/{name}`: Retrieves a media list by owner and name. Aggregates data from MDBList and TMDB.
- `GET /media/poster/{tmdbId}`: Fetches the poster image from TMDB using the provided TMDB ID.

## Stremio Endpoints
- `GET /stremio/manifest.json`: Returns the Stremio add-on manifest.
- `GET /stremio/catalog/movie/slasher/{franchise}.json`: Retrieves the Slasher Films catalog from MDBList, with optional filtering by franchise. Supported franchises include:
    - Halloween
    - Friday the 13th
    - A Nightmare on Elm Street
    - Texas Chainsaw Massacre
    - Scream
    - Child's Play
    - Terrifier
    - Hatchet
    - Thanksgiving
    - Black Christmas
- `GET /stremio/catalog/movie/gangster.json`: Retrieves the Gangster Movies catalog from MDBList.