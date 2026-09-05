using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Vyzor.Application.Interfaces;
using Vyzor.Infrastructure.Mongo;
using Vyzor.Infrastructure.Options;

namespace Vyzor.Infrastructure.Services;

public class TechnicalLogService : ITechnicalLogService
{
    private const string DefaultDatabaseName = "vyzor_technical";
    private const string CollectionName = "technical_logs";

    private readonly IMongoCollection<TechnicalLogDocument> _logs;

    public TechnicalLogService(
        IMongoClient mongoClient,
        IOptions<MongoOptions> options)
    {
        var databaseName = string.IsNullOrWhiteSpace(
            options.Value.DatabaseName)
            ? DefaultDatabaseName
            : options.Value.DatabaseName;

        var database = mongoClient.GetDatabase(databaseName);

        _logs = database.GetCollection<TechnicalLogDocument>(
            CollectionName);
    }

    public async Task LogAsync(
        string level,
        string message,
        string? path = null,
        string? userId = null,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var document = new TechnicalLogDocument
        {
            Level = level,
            Message = message,
            Path = path,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            Details = details
        };

        await _logs.InsertOneAsync(
            document,
            cancellationToken: cancellationToken);
    }
}