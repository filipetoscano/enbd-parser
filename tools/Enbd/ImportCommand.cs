using Dapper;
using Enbd.StatementParser;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Enbd;

/// <summary />
[Command( "import", Description = "Import JSON format into SQLite database" )]
public class ImportCommand
{
    /// <summary />
    public ImportCommand()
    {
    }


    /// <summary />
    [Argument( 0, Description = "Pattern" )]
    public string Pattern { get; set; } = "*.json";

    /// <summary />
    [Option( "-d|--directory", CommandOptionType.SingleValue, Description = "" )]
    public string SourcePath { get; set; } = Environment.CurrentDirectory;


    /// <summary />
    public int OnExecute()
    {
        /*
         * 
         */
        var m = new Matcher();
        m.AddInclude( this.Pattern );

        var res = m.Execute( new DirectoryInfoWrapper( new DirectoryInfo( this.SourcePath ) ) );
        var files = res.Files.Select( x => Path.Combine( this.SourcePath, x.Path ) );


        /*
         * 
         */
        var sql = new
        {
            Schema = @"create table if not exists TxRow
(
  ObjNumber text,
  ObjType text,
  TransactionDate text,
  TransactionType text,
  Operation text,
  Amount number,
  Description text,
  ForeignAmount number,
  ForeignCurrency number,
  Balance number
);",

            DeleteAll = @"delete from TxRow",

            InsertRow = @"insert into TxRow
(
  ObjNumber, ObjType, TransactionDate, TransactionType,
  Operation, Amount, Description, ForeignAmount,
  ForeignCurrency, Balance
)
values
(
  @ObjNumber, @ObjType, @TransactionDate, @TransactionType,
  @Operation, @Amount, @Description, @ForeignAmount,
  @ForeignCurrency, @Balance
);",
        };


        /*
         * 
         */
        var conn = new SqliteConnection( "Data Source=hello.db" );
        conn.Open();

        conn.Execute( sql.Schema );

        var tx = conn.BeginTransaction();
        conn.Execute( sql.DeleteAll );


        /*
         * 
         */
        foreach ( var f in files )
        {
            Console.WriteLine( f );

            var json = File.ReadAllText( f );
            var obj = JsonSerializer.Deserialize<EnbdStatement>( json );

            if ( obj is CreditCardStatement cc )
            {
                foreach ( var row in cc.Transactions )
                {
                    Console.Write( "." );

                    conn.Execute( sql.InsertRow, new
                    {
                        ObjNumber = cc.CardNumber,
                        ObjType = "CC",
                        TransactionDate = row.TransactionDate.ToString( "yyyy-MM-dd" ),
                        TransactionType = row.TransactionType.ToString(),
                        Operation = row.Operation?.ToString(),
                        row.Amount,
                        row.Description,
                        row.ForeignAmount,
                        row.ForeignCurrency,
                        Balance = default( decimal? ),
                    }, transaction: tx );
                }

                Console.WriteLine();
            }

            if ( obj is CurrentAccountStatement ca )
            {
                foreach ( var row in ca.Transactions )
                {
                    Console.Write( "." );

                    conn.Execute( sql.InsertRow, new
                    {
                        ObjNumber = ca.AccountNumber,
                        ObjType = "CA",
                        TransactionDate = row.TransactionDate.ToString( "yyyy-MM-dd" ),
                        TransactionType = row.TransactionType.ToString(),
                        Operation = row.Operation?.ToString(),
                        row.Amount,
                        row.Description,
                        row.ForeignAmount,
                        row.ForeignCurrency,
                        row.Balance,
                    }, transaction: tx );
                }

                Console.WriteLine();
            }
        }

        tx.Commit();


        /*
         * 
         */
        return 0;
    }
}
