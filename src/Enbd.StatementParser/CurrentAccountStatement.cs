namespace Enbd.StatementParser;

/// <summary />
public class CurrentAccountStatement
{
    /// <summary />
    public string AccountNumber { get; set; } = default!;

    /// <summary />
    public List<CurrentAccountTransaction> Transactions { get; set; } = new List<CurrentAccountTransaction>();
}


/// <summary />
public class CurrentAccountTransaction
{
    /// <summary />
    public DateOnly TransactionDate { get; set; }

    /// <summary />
    public string Description { get; set; } = default!;

    /// <summary />
    public decimal Amount { get; set; }

    /// <summary />
    public TransactionType TransactionType { get; set; }
}