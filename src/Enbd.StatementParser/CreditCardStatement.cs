using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
public class CreditCardStatement : EnbdStatement
{
    /// <summary />
    public string CardNumber { get; set; } = default!;

    /// <summary />
    public List<CreditCardTransaction> Transactions { get; set; } = new List<CreditCardTransaction>();
}


/// <summary />
public class CreditCardTransaction
{
    /// <summary />
    public DateOnly TransactionDate { get; set; }

    /// <summary />
    public DateOnly PostingDate { get; set; }

    /// <summary />
    public TransactionType TransactionType { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public CreditCardOperation? Operation { get; set; }

    /// <summary />
    public string Description { get; set; } = default!;

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Country { get; set; }

    /// <summary />
    public decimal Amount { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? ForeignCurrency { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public decimal? ForeignAmount { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public decimal? ForeignExchange { get; set; }
}