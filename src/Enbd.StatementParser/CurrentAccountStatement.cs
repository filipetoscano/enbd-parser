using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
public class CurrentAccountStatement : EnbdStatement
{
    /// <summary />
    public string AccountNumber { get; set; } = "";

    /// <summary />
    public decimal BalanceOpening { get; set; }

    /// <summary />
    public decimal BalanceClosing { get; set; }

    /// <summary />
    public List<CurrentAccountTransaction> Transactions { get; set; } = new List<CurrentAccountTransaction>();
}


/// <summary />
public class CurrentAccountTransaction
{
    /// <summary />
    public DateOnly TransactionDate { get; set; }

    /// <summary />
    public TransactionType TransactionType { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public CurrentAccountOperation? Operation { get; set; }

    /// <summary />
    public string Description { get; set; } = default!;

    /// <summary />
    public decimal Amount { get; set; }

    /// <summary />
    public decimal Balance { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? ForeignCurrency { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public decimal? ForeignAmount { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? RelatedTo { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Country { get; set; }

    /// <summary />
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<string>? Lines { get; set; } = new List<string>();


    /// <summary />
    public string? TryLine( int index )
    {
        if ( this.Lines == null )
            return null;

        if ( index >= this.Lines.Count )
            return null;

        return this.Lines[ index ];
    }


    /// <summary />
    public string LineAt( int index )
    {
        if ( this.Lines == null )
            throw new InvalidOperationException();

        if ( index >= this.Lines.Count )
            throw new InvalidOperationException();

        return this.Lines[ index ];
    }
}