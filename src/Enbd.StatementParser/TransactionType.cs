using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum TransactionType
{
    /// <summary />
    Credit,

    /// <summary />
    Debit,
}