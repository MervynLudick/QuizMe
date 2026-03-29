using Azure.Storage.Blobs;
using System.Text.Json;

namespace api;

public class QuestionService
{
    private readonly string _connectionString;
    private BlobClient? _blobClient;

    public QuestionService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private BlobClient BlobClient => _blobClient ??= new BlobServiceClient(_connectionString)
        .GetBlobContainerClient("default")
        .GetBlobClient("question.json");

    public async Task<List<Question>> GetRandomQuestionsAsync(int count = 20)
    {
        var response = await BlobClient.DownloadContentAsync();
        var all = JsonSerializer.Deserialize<List<Question>>(
            response.Value.Content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        return [.. all.OrderBy(_ => Random.Shared.Next()).Take(count)];
    }
}
