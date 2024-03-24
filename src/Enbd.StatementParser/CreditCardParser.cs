using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Enbd.StatementParser;

/// <summary />
public class CreditCardParser
{
    /// <summary />
    public CreditCardStatement Parse( FileContent content )
    {
        var stmt = new CreditCardStatement();


        /*
         * Round #1
         * Extract only the interesting transaction lines.
         */
        var fc = new FileContent();
        var reg = new Regex( @"Page \d+ of \d+$" );
        var ccn = new Regex( @"\d{4} XXXX XXXX \d{4}" );

        for ( var j = 0; j < content.Pages.Count; j++ )
        {
            var lines = content.Pages[ j ].Text.Split( "\n" );

            var active = false;
            var sb = new StringBuilder();

            for ( var i = 0; i < lines.Length; i++ )
            {
                var l = lines[ i ].Trim();

                if ( active == false )
                {
                    if ( j == 0 )
                    {
                        if ( l == "Primary Card Number" )
                        {
                            var lxr = ccn.Match( lines[ i + 1 ] );

                            if ( lxr.Success == true )
                                stmt.CardNumber = lxr.Value.Replace( " ", "-" );

                            i++;
                            active = true;
                            continue;
                        }
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
                else
                {
                    if ( l == "Credit Card Statement" )
                    {
                        active = false;
                        continue;
                    }

                    sb.AppendLine( l );
                }
            }

            fc.Pages.Add( new PageContent()
            {
                Number = j,
                Text = sb.ToString(),
            } );
        }


        /*
         * Round #2
         */
        var reg1 = new Regex( @"^(?<txdate>\d+/\d+/\d+) (?<post>\d+/\d+/\d+) (?<desc>.*?) (?<amount>(\d+,)*\d+\.\d+)(?<dir>CR)?(?<currency> [A-Z]*)?$" );
        var reg2 = new Regex( @"^(\*)?\(1 AED = [A-Z]{3} (?<xr>\d+.\d+)\)$" );
        var reg3 = new Regex( @"^(?<amount>(\d+,)*\d+\.\d+)(?<dir>CR)?$" );

        var jso = new JsonSerializerOptions() { WriteIndented = true };

        foreach ( var p in fc.Pages )
        {
            var lines = p.Text.Split( Environment.NewLine );

            for ( var i = 0; i < lines.Length; i++ )
            {
                if ( lines[ i ].Length == 0 )
                    continue;

                var m1 = reg1.Match( lines[ i ] );

                if ( m1.Success == false )
                {
                    Console.WriteLine( "FAIL: '{0}'", lines[ i ] );
                    continue;
                }


                /*
                 * 
                 */
                var tx = new CreditCardTransaction();
                tx.TransactionDate = DateOnly.ParseExact( m1.Groups[ "txdate" ].Value, "dd/MM/yyyy" );
                tx.PostingDate = DateOnly.ParseExact( m1.Groups[ "post" ].Value, "dd/MM/yyyy" );
                tx.Description = m1.Groups[ "desc" ].Value.Trim();
                tx.Amount = decimal.Parse( m1.Groups[ "amount" ].Value );
                tx.TransactionType = TransactionType.Debit;

                if ( m1.Groups[ "dir" ].Value == "CR" )
                    tx.TransactionType = TransactionType.Credit;

                stmt.Transactions.Add( tx );


                /*
                 * Foreign?
                 */
                var forex = m1.Groups[ "currency" ].Value.Trim();

                if ( forex.Length > 0 )
                {
                    var xrLine = lines[ i + 1 ];
                    var amountLine = lines[ i + 2 ];

                    var m2 = reg2.Match( xrLine );
                    var m3 = reg3.Match( amountLine );

                    if ( m2.Success == false )
                    {
                        Console.WriteLine( "FAIL/X: '{0}'", xrLine );
                        continue;
                    }

                    if ( m3.Success == false )
                    {
                        Console.WriteLine( "FAIL/Y: '{0}'", amountLine );
                        continue;
                    }

                    tx.ForeignCurrency = forex;
                    tx.ForeignAmount = tx.Amount;
                    tx.ForeignExchange = decimal.Parse( m2.Groups[ "xr" ].Value );
                    tx.Amount = decimal.Parse( m3.Groups[ "amount" ].Value );

                    if ( m3.Groups[ "dir" ].Value == "CR" )
                        tx.TransactionType = TransactionType.Credit;

                    i += 2;
                }


                /*
                 * Infer operation based on TransactionType/Description.
                 */
                if ( tx.TransactionType == TransactionType.Credit )
                {
                    if ( tx.Description == "TRANSFER PAYMENT RECEIVED THANK YOU" )
                        tx.Operation = CreditCardOperation.CreditCardRepayment;
                    else
                        tx.Operation = CreditCardOperation.Reversal;
                }
                else
                {
                    tx.Operation = CreditCardOperation.Payment;
                }
            }
        }

        return stmt;
    }
}