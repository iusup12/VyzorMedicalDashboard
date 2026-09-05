using System;
using System.Collections.Generic;
using System.Text;
namespace Vyzor.Domain.Common;

public abstract class Entity
{
    public int Id { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}