using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace api;

public class SubmitAnswerFunction(GameStorageService storage)
{
    [Function("SubmitAnswer")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "games/{roomCode}/answer")] HttpRequest req,
        string roomCode)
    {
        var body = await JsonSerializer.DeserializeAsync<SubmitAnswerRequest>(req.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (body is null)
            return new BadRequestObjectResult("Invalid request.");

        var game = await storage.GetGameAsync(roomCode);
        if (game is null)
            return new NotFoundObjectResult("Game not found.");

        if (game.IsGameOver)
            return new BadRequestObjectResult("Game is already over.");

        var player = game.Players.FirstOrDefault(p => p.Name == body.PlayerName);
        if (player is null)
            return new NotFoundObjectResult("Player not in this game.");

        try
        {
            game.SubmitAnswer(player, body.Answer, body.QuestionNumber);
        }
        catch (InvalidOperationException ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }

        await storage.SaveGameAsync(roomCode, game);
        return new OkObjectResult(game);
    }
}

public record SubmitAnswerRequest(string PlayerName, string Answer, int QuestionNumber);
