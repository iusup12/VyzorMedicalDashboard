using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Infrastructure.Options;

public class MongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;
}
