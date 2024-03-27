using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum CurrentAccountOperation
{
    /// <summary>
    /// Payment of the due amount on a credit card.
    /// </summary>
    CardRepayment = 1,

    /// <summary>
    /// Purchase using a debit card.
    /// </summary>
    CardPurchase,

    /// <summary>
    /// Refund of a prior purchase using a debit card.
    /// </summary>
    CardRefund,

    /// <summary>
    /// Constitution of a term deposit.
    /// </summary>
    TermDepositCreate,

    /// <summary>
    /// Maturity or (partial) redemption from term deposit.
    /// </summary>
    TermDepositMaturity,

    /// <summary />
    InterestEarned,

    /// <summary>
    /// Transfer of (monthly) salary.
    /// </summary>
    Salary,

    /// <summary>
    /// Cash withdrawal from an ATM.
    /// </summary>
    CashWithdrawal,

    /// <summary>
    /// Cash deposit into an ATM.
    /// </summary>
    CashDeposit,

    /// <summary>
    /// Payment through a cheque.
    /// </summary>
    Cheque,

    /// <summary>
    /// Bank transfer, to another account within same Bank.
    /// </summary>
    BankTransfer,

    /// <summary>
    /// Domestic transfer, to an account at another Bank.
    /// </summary>
    DomesticTransfer,

    /// <summary>
    /// Foreign transfer, to an account in another country.
    /// </summary>
    ForeignTransfer,
}