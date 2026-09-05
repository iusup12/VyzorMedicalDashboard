using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.Interfaces;

public interface ITechnicalLogService
{
    Task LogAsync(
        string level,
        string message,
        string? path = null,
        string? userId = null,
        string? details = null,
        CancellationToken cancellationToken = default);
}
