# VideoGames API

ASP.NET Core Web API for the existing SQL Server `VideoGames` database and `dbo.Games` table. The API uses Dapper and does not create or modify the database schema.

## Configure the connection string

The tracked `appsettings.json` contains a local, Windows-authenticated example without a password. Set the actual SQL Server instance and credentials outside source control.

### User Secrets (local development)

From the project directory:

```powershell
dotnet user-secrets set "ConnectionStrings:VideoGamesConnection" "Server=YOUR_SERVER;Database=VideoGames;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

For Windows authentication, use a connection string such as:

```powershell
dotnet user-secrets set "ConnectionStrings:VideoGamesConnection" "Server=YOUR_SERVER;Database=VideoGames;Trusted_Connection=True;TrustServerCertificate=True;"
```

Do not commit passwords, access tokens, or production connection strings.

### Environment variable

Alternatively, set the hierarchical configuration key using double underscores:

```powershell
$env:ConnectionStrings__VideoGamesConnection = "Server=YOUR_SERVER;Database=VideoGames;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

## Run the API

```powershell
dotnet restore
dotnet run
```

When running in Development, Swagger UI is available at `/swagger`, and the OpenAPI document is available at `/swagger/v1/swagger.json`. CORS permits browser clients hosted on `localhost` and `127.0.0.1` during development only.

## Endpoints

- `GET /api/games` — list all games
- `GET /api/games/{id}` — get a game by `GameId`
- `POST /api/games` — create a game
- `PUT /api/games/{id}` — update a game
- `DELETE /api/games/{id}` — delete a game

Create and update requests use this JSON shape:

```json
{
  "title": "Example Game",
  "releaseYear": 2024,
  "genre": "Adventure"
}
```

`title` is required and limited to 200 characters. `releaseYear` is optional but must be a four-digit year. `genre` is optional and limited to 100 characters. Invalid requests return `400`; missing games return `404`.
