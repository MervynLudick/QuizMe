using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace api;

public class GetGameFunction(GameStorageService storage)
{
    [Function("GetGame")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{roomCode}")] HttpRequest req,
        string roomCode)
    {
        var game = await storage.GetGameAsync(roomCode);
        if (game is null)
            return new NotFoundObjectResult("Game not found.");

        return new OkObjectResult(game);
    }
}
