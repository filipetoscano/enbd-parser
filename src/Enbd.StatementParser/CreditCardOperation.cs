using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CreditCardOperation
{
    /// <summary />
    CreditCardRepayment = 1,

    /// <summary />
    Payment,

    /// <summary />
    Reversal,
}