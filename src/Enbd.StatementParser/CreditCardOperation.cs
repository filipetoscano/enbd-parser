using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CreditCardOperation
{
    /// <summary />
    CardRepayment = 1,

    /// <summary />
    CardPurchase,

    /// <summary />
    CardRefund,
}