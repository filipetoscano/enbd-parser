using McMaster.Extensions.CommandLineUtils;

namespace Enbd;

/// <summary />
[Command( "enbd", Description = "" )]
[Subcommand( typeof( ParseCommand ) )]
internal class Program
{
    /// <summary />
    internal static int Main( string[] args )
    {
        try
        {
            return CommandLineApplication.Execute<Program>( args );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex.ToString() );
            return 1;
        }
    }
}