using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CreditCardOperation
{
    /// <summary>
    /// Payment of the due amount on a credit card.
    /// </summary>
    CardRepayment = 1,

    /// <summary>
    /// Purchase using a credit card.
    /// </summary>
    CardPurchase,

    /// <summary>
    /// Refund of a prior purchase using a credit card.
    /// </summary>
    CardRefund,
}