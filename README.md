# MediaAPI
MediaAPI is a .NET Web API that aggregates media data from external sources like MDBList and TMDB.

## Endpoints
- `GET /list/{owner}/{name}`: Retrieves a media list by owner and name. Aggregates data from MDBList and TMDB.
- `GET /poster/{tmdbId}`: Fetches the poster image from TMDB using the provided TMDB ID.