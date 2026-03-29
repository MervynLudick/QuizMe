using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace api;

public class CreateGameFunction(GameStorageService storage, QuestionService questions)
{
    [Function("CreateGame")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "games")] HttpRequest req)
    {
        var body = await JsonSerializer.DeserializeAsync<CreateGameRequest>(req.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (body is null || body.NumberOfPlayers < 2)
            return new BadRequestObjectResult("Provide numberOfPlayers (≥2).");

        var gameQuestions = await questions.GetRandomQuestionsAsync(20);
        var game = new Game(body.NumberOfPlayers) { Questions = gameQuestions };
        var roomCode = await storage.CreateGameAsync(game);
        return new OkObjectResult(new { roomCode });
    }
}

public record CreateGameRequest(int NumberOfPlayers);
