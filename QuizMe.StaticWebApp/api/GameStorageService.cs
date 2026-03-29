using Azure;
using Azure.Data.Tables;
using System.Text.Json;

namespace api;

public class GameStorageService
{
    private readonly string _connectionString;
    private TableClient? _tableClient;
    private const string PartitionKey = "Games";

    public GameStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private TableClient TableClient
    {
        get
        {
            if (_tableClient == null)
            {
                var client = new TableServiceClient(_connectionString).GetTableClient("Games");
                client.CreateIfNotExists();
                _tableClient = client;
            }
            return _tableClient;
        }
    }

    public async Task<Game?> GetGameAsync(string roomCode)
    {
        try
        {
            var response = await TableClient.GetEntityAsync<GameEntity>(PartitionKey, roomCode);
            return JsonSerializer.Deserialize<Game>(response.Value.GameJson);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<string> CreateGameAsync(Game game)
    {
        var roomCode = GenerateRoomCode();
        var entity = new GameEntity
        {
            PartitionKey = PartitionKey,
            RowKey = roomCode,
            GameJson = JsonSerializer.Serialize(game)
        };
        await TableClient.AddEntityAsync(entity);
        return roomCode;
    }

    public async Task SaveGameAsync(string roomCode, Game game)
    {
        var entity = new GameEntity
        {
            PartitionKey = PartitionKey,
            RowKey = roomCode,
            GameJson = JsonSerializer.Serialize(game)
        };
        await TableClient.UpsertEntityAsync(entity);
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Range(0, 4)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }
}
