using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace api;

public class JoinGameFunction(GameStorageService storage)
{
    [Function("JoinGame")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "games/{roomCode}/join")] HttpRequest req,
        string roomCode)
    {
        var body = await JsonSerializer.DeserializeAsync<JoinGameRequest>(req.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (body is null || string.IsNullOrWhiteSpace(body.PlayerName))
            return new BadRequestObjectResult("playerName is required.");

        var game = await storage.GetGameAsync(roomCode);
        if (game is null)
            return new NotFoundObjectResult("Game not found.");

        if (game.IsGameOver)
            return new BadRequestObjectResult("Game is already over.");

        if (game.Players.Any(p => p.Name == body.PlayerName))
            return new ConflictObjectResult("Player name already taken.");

        if (game.Players.Count >= game.NumberOfPlayers)
            return new BadRequestObjectResult("Game is full.");

        game.Players.Add(new Player { Name = body.PlayerName });
        await storage.SaveGameAsync(roomCode, game);
        return new OkObjectResult(new { playerName = body.PlayerName });
    }
}

public record JoinGameRequest(string PlayerName);
