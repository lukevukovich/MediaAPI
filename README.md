# MediaAPI
MediaAPI is a .NET Web API that aggregates media data from external sources like MDBList and TMDB.

## Media Endpoints
- `GET /media/list/{owner}/{name}`: Retrieves a media list by owner and name. Aggregates data from MDBList and TMDB.
- `GET /media/poster/{tmdbId}`: Fetches the poster image from TMDB using the provided TMDB ID.

## Stremio Endpoints
- `GET /stremio/manifest`: Returns the Stremio add-on manifest.
- `GET /stremio/catalog/{type}/{id}`: Provides a media catalog for Stremio based on type and ID.