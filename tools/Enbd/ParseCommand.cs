using Enbd.StatementParser;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Text.Json;

namespace Enbd;

/// <summary />
[Command( "parse", Description = "Parse PDF files into JSON format" )]
public class ParseCommand
{
    /// <summary />
    public ParseCommand()
    {
    }


    /// <summary />
    [Argument( 0, Description = "Pattern" )]
    public string Pattern { get; set; } = "*.pdf";

    /// <summary />
    [Option( "-d|--directory", CommandOptionType.SingleValue, Description = "" )]
    public string SourcePath { get; set; } = Environment.CurrentDirectory;

    /// <summary />
    [Option( "-o|--output", CommandOptionType.SingleValue, Description = "" )]
    public string OutputPath { get; set; } = Environment.CurrentDirectory;

    /// <summary />
    [Option( "-p|--password", CommandOptionType.SingleValue, Description = "Password" )]
    public string? Password { get; set; } = Environment.GetEnvironmentVariable( "ENBD_PASSWORD" );



    /// <summary />
    public int OnExecute()
    {
        var te = new TextExtractor();
        var cc = new ContentClassifier();
        var ccp = new CreditCardParser();
        var cap = new CurrentAccountParser();


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
        Directory.CreateDirectory( this.OutputPath );


        /*
         * 
         */
        var jso = new JsonSerializerOptions() { WriteIndented = true };


        /*
         * 
         */
        foreach ( var pdf in files )
        {
            var fn = Path.GetFileName( pdf );
            Console.WriteLine( fn );

            var fc = te.Extract( pdf, this.Password );
            var stmt = cc.Classify( fc );

            if ( stmt == null )
                continue;


            if ( stmt == Statement.CreditCard )
            {
                var scc = ccp.Parse( fc );

                var min = scc.Transactions.Min( x => x.TransactionDate );
                var fname = scc.CardNumber + "-" + min.Year + "-" + min.Month.ToString( "00" ) + ".json";
                var oname = Path.Combine( this.OutputPath, fname );

                var json = JsonSerializer.Serialize( scc, jso );
                File.WriteAllText( oname, json );
            }

            if ( stmt == Statement.CurrentAccount )
            {
                var sca = cap.Parse( fc );

                var min = sca.Transactions.Max( x => x.TransactionDate );
                var fname = sca.AccountNumber + "-" + min.Year + "-" + min.Month.ToString( "00" ) + ".json";
                var oname = Path.Combine( this.OutputPath, fname );

                var json = JsonSerializer.Serialize( sca, jso );
                File.WriteAllText( oname, json );
            }
        }


        return 0;
    }
}