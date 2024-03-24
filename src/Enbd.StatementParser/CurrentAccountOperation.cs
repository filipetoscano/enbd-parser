using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CurrentAccountOperation
{
    /// <summary />
    CreditCardRepayment = 1,

    /// <summary />
    CardPurchase,

    /// <summary />
    ToTermDeposit,

    /// <summary />
    FromTermDeposit,

    /// <summary />
    InterestEarned,

    /// <summary />
    Salary,

    /// <summary />
    CashWithdrawal,

    /// <summary />
    CashDeposit,

    /// <summary />
    Cheque,
}