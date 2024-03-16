using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum TransactionInfo
{
    /// <summary />
    CreditCardRepayment = 1,

    /// <summary />
    ToTermDeposit,

    /// <summary />
    FromTermDeposit,
}