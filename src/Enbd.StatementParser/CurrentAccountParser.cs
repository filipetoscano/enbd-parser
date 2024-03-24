using System.Text;
using System.Text.RegularExpressions;

namespace Enbd.StatementParser;

/// <summary />
public class CurrentAccountParser
{
    /// <summary />
    public CurrentAccountStatement Parse( FileContent content )
    {
        var stmt = new CurrentAccountStatement();


        /*
         * Round #1
         */
        var fc = new FileContent();
        var reg = new Regex( @"^Page \d+ of \d+$" );

        for ( var pn = 0; pn < content.Pages.Count; pn++ )
        {
            var lines = content.Pages[ pn ].Text.Split( "\n" );

            var active = false;
            var sb = new StringBuilder();

            for ( var i = 0; i < lines.Length; i++ )
            {
                var l = lines[ i ].Trim();

                if ( l.Length == 0 )
                    continue;

                if ( pn == 0 && l == "CURRENT ACCOUNT" )
                {
                    stmt.AccountNumber = lines[ i + 1 ].Trim();
                    continue;
                }

                if ( active == true )
                {
                    sb.AppendLine( l );
                    continue;
                }
                else
                {
                    if ( reg.IsMatch( l ) == true )
                    {
                        active = true;
                        continue;
                    }
                }
            }

            if ( sb.Length > 0 )
            {
                fc.Pages.Add( new PageContent()
                {
                    Number = pn,
                    Text = sb.ToString(),
                } );
            }
        }


        /*
         *
         */
        var regex = new
        {
            BroughtForward = new Regex( @"^\d{2}[A-Z]{3}\d{2} BROUGHT FORWARD (?<balance>.*?)(Cr)?$" ),
            CarriedForward = new Regex( @"^CARRIED FORWARD (?<balance>.*?)Cr$" ),
            TxStart = new Regex( @"^(?<date>\d{2}[A-Z]{3}\d{2}) (?<line>.*?)$" ),
            TxEnd = new Regex( @"(?<line>.*?) (?<amount>[0-9,]+\.[0-9]{2}) (?<balance>[0-9,]+\.[0-9]{2})Cr$" ),

            Country = new Regex( @"(.*?):[A-Z]{2}" ),
            Amount = new Regex( @"(?<amount>[0-9,]+\.[0-9]{2}),(?<currency>[A-Z]{3})" ),
        };


        /*
         * 
         */
        var openingBalance = 0.0m;
        var closingBalance = 0.0m;
        CurrentAccountTransaction? tx = null;

        foreach ( var p in fc.Pages )
        {
            var lines = p.Text.Split( "\n" );

            for ( var i = 0; i < lines.Length; i++ )
            {
                var line = lines[ i ].Trim();


                /*
                 * 
                 */
                if ( line.Length == 0 )
                    continue;


                /*
                 * 
                 */
                var m2 = regex.BroughtForward.Match( line );

                if ( m2.Success == true )
                {
                    if ( p.Number == 0 )
                        openingBalance = decimal.Parse( m2.Groups[ "balance" ].Value );

                    tx = null;
                    continue;
                }


                /*
                 * 
                 */
                var m1 = regex.CarriedForward.Match( line );

                if ( m1.Success == true )
                {
                    closingBalance = decimal.Parse( m1.Groups[ "balance" ].Value );

                    tx = null;
                    continue;
                }


                /*
                 * 
                 */
                var m3 = regex.TxStart.Match( line );

                if ( m3.Success == true )
                {
                    var date = ParseDate( m3.Groups[ "date" ].Value );
                    var rest = m3.Groups[ "line" ].Value;

                    tx = new CurrentAccountTransaction()
                    {
                        TransactionDate = date,
                    };

                    tx.Lines!.Add( rest );

                    stmt.Transactions.Add( tx );

                    continue;
                }


                /*
                 * 
                 */
                if ( tx == null )
                {
                    Console.WriteLine( "FAIL: {0}: {1}", i, line );
                    continue;
                }


                /*
                 * 
                 */
                var m4 = regex.TxEnd.Match( line );

                if ( m4.Success == true )
                {
                    var rest = m4.Groups[ "line" ].Value;
                    var amount = decimal.Parse( m4.Groups[ "amount" ].Value );
                    var balance = decimal.Parse( m4.Groups[ "balance" ].Value );

                    tx.Amount = amount;
                    tx.Balance = balance;
                    tx.Lines!.Add( rest );

                    continue;
                }


                /*
                 * 
                 */
                tx.Lines!.Add( line );
            }
        }


        /*
         * 
         */
        stmt.BalanceOpening = openingBalance;
        stmt.BalanceClosing = closingBalance;


        /*
         * Credit/Debit
         */
        var ftx = (CurrentAccountTransaction) stmt.Transactions[ 0 ];
        ftx.TransactionType = ( ftx.Balance > stmt.BalanceOpening )
            ? TransactionType.Credit : TransactionType.Debit;

        for ( var i = 1; i < stmt.Transactions.Count; i++ )
        {
            ftx = (CurrentAccountTransaction) stmt.Transactions[ i ];
            var prev = (CurrentAccountTransaction) stmt.Transactions[ i - 1 ];

            ftx.TransactionType = ( ftx.Balance > prev.Balance )
                ? TransactionType.Credit : TransactionType.Debit;
        }


        /*
         * 
         */
        foreach ( CurrentAccountTransaction itx in stmt.Transactions )
        {
            /*
             * 
             */
            if ( itx.TryLine( 0 ) == "CREDIT CARD PAYMENT" )
            {
                itx.Operation = CurrentAccountOperation.CreditCardRepayment;
                itx.RelatedTo = itx.LineAt( 1 ).Substring( "CC NO.-".Length );
                itx.Description = "Credit Card Payment";

                continue;
            }

            if ( itx.TryLine( 0 ) == "POS-PURCHASE" )
            {
                itx.Operation = CurrentAccountOperation.CardPurchase;
                itx.RelatedTo = Cardify( itx.LineAt( 1 ).Substring( "CARD NO. ".Length ) );
                itx.Description = itx.LineAt( 2 );

                var d = regex.Country.Match( itx.LineAt( 3 ) );

                if ( d.Success == true )
                    itx.Description += " " + d.Value;

                var am = regex.Amount.Match( itx.LineAt( 3 ) );

                if ( am.Success == true && am.Groups[ "currency" ].Value != "AED" )
                {
                    itx.ForeignAmount = decimal.Parse( am.Groups[ "amount" ].Value );
                    itx.ForeignCurrency = am.Groups[ "currency" ].Value;
                }
            }

            if ( itx.TryLine( 0 ) == "POS-REFUNDS/RET" )
            {
                itx.Operation = CurrentAccountOperation.CardRefund;
                itx.RelatedTo = Cardify( itx.LineAt( 1 ).Substring( "CARD NO. ".Length ) );
                itx.Description = itx.LineAt( 2 );
            }


            /*
             * Transfers
             */
            if ( itx.TryLine( 0 ) == "BANKNET TRANSFER" )
            {
                itx.Operation = CurrentAccountOperation.BankTransfer;
                itx.Description = itx.LineAt( 1 );

                continue;
            }

            if ( itx.TryLine( 0 ) == "TRANSFER" )
            {
                itx.Operation = CurrentAccountOperation.DomesticTransfer;
                itx.Description = string.Join( " ", itx.Lines![ 1.. ] );

                continue;
            }


            /*
             * Salary In
             */
            if ( itx.TryLine( 0 ) == "SALARY CREDIT" )
            {
                itx.Operation = CurrentAccountOperation.Salary;
                itx.Description = itx.LineAt( 1 ) + " " + itx.LineAt( 2 );

                continue;
            }


            /*
             * Cheque Out
             */
            if ( itx.TryLine( 0 ) == "INWARD CLEARING  CHQ. NO:" )
            {
                itx.Operation = CurrentAccountOperation.Cheque;
                itx.RelatedTo = itx.Lines![ 2 ].Substring( "CHQ. NO: ".Length );
                itx.Description = itx.Lines![ 2 ];

                continue;
            }


            /*
             * Cash
             */
            if ( itx.TryLine( 0 ) == "SDM DEPOSIT CR" )
            {
                itx.Operation = CurrentAccountOperation.CashDeposit;
                itx.Description = itx.LineAt( 2 ) + " " + itx.LineAt( 3 );

                continue;
            }

            if ( itx.TryLine( 0 ) == "DR ATM TRANSACTION" )
            {
                itx.Operation = CurrentAccountOperation.CashWithdrawal;
                itx.RelatedTo = Cardify( itx.LineAt( 1 ).Substring( "CARD NO. ".Length ) );
                itx.Description = itx.LineAt( 3 );

                continue;
            }

            if ( itx.TryLine( 0 ) == "UAE SWITCH WDL" )
            {
                // Cash Withdrawl in other Bank ATM

                itx.Operation = CurrentAccountOperation.CashWithdrawal;
                itx.RelatedTo = Cardify( itx.LineAt( 1 ).Substring( "CARD NO. ".Length ) );
                itx.Description = string.Join( " ", itx.Lines![ 2.. ] );

                continue;
            }


            /*
             * Term Deposit
             */
            if ( itx.TryLine( 0 ) == "DR. TRAN FOR FUNDING A/C" )
            {
                itx.Operation = CurrentAccountOperation.ToTermDeposit;
                itx.RelatedTo = itx.LineAt( 1 );
                itx.Description = "Term Deposit Constitution";
                itx.Lines = null;

                continue;
            }

            if ( itx.TryLine( 1 ) == "PROCEEDS  CREDIT TO" )
            {
                itx.Operation = CurrentAccountOperation.FromTermDeposit;
                itx.RelatedTo = itx.Lines![ 0 ].Substring( 0, 19 );
                itx.Description = "Term Deposit Maturity";

                continue;
            }

            if ( itx.TryLine( 3 ) == "P.A. INTEREST RUN" )
            {
                itx.Operation = CurrentAccountOperation.InterestEarned;
                itx.RelatedTo = itx.LineAt( 0 ).Substring( 0, 19 );
                itx.Description = itx.LineAt( 2 );

                continue;
            }


            /*
             * 
             */
            itx.Description = itx.LineAt( 0 );
        }

        return stmt;
    }


    /// <summary />
    private string Cardify( string v )
    {
        if ( v.Length == 16 )
        {
            v = v.Substring( 0, 4 )
                + "-" + v.Substring( 4, 4 )
                + "-" + v.Substring( 8, 4 )
                + "-" + v.Substring( 12, 4 );
        }

        return v;
    }


    /// <summary />
    private DateOnly ParseDate( string value )
    {
        var dd = int.Parse( value.Substring( 0, 2 ) );
        var MM = ToMonth( value.Substring( 2, 3 ) );
        var yy = int.Parse( value.Substring( 5, 2 ) ) + 2000;

        return new DateOnly( yy, MM, dd );
    }

    private static readonly string[] Months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };


    /// <summary />
    private static int ToMonth( string value )
    {
        return Array.IndexOf( Months, value ) + 1;
    }
}