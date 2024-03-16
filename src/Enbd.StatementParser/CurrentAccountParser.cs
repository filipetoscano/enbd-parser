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

        for ( var j = 0; j < content.Pages.Count; j++ )
        {
            var lines = content.Pages[ j ].Text.Split( "\n" );

            var active = false;
            var sb = new StringBuilder();

            for ( var i = 0; i < lines.Length; i++ )
            {
                var l = lines[ i ].Trim();



            }
        }

        return stmt;
    }
}
