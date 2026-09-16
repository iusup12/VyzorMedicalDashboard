
using Vyzor.Infrastructure.Mongo;

namespace Vyzor.Infrastructure.Interfaces;

public interface ITechnicalLogService
{
    Task LogAsync(
        string level,
        string message,
        string? path = null,
        string? userId = null,
        string? details = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TechnicalLogDocument>> GetAllAsync(
        CancellationToken cancellationToken = default);
}

