using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CurrentAccountOperation
{
    /// <summary />
    CardRepayment = 1,

    /// <summary />
    CardPurchase,

    /// <summary />
    CardRefund,

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

    /// <summary />
    BankTransfer,

    /// <summary />
    DomesticTransfer,

    /// <summary />
    ForeignTransfer,
}