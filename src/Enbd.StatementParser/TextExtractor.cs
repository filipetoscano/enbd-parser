using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Enbd.StatementParser;

/// <summary />
public class TextExtractor
{
    /// <summary />
    public FileContent Extract( string fileName, string? password )
    {
        var fc = new FileContent()
        {
            Filename = fileName,
        };


        /*
         * 
         */
        var po = new ParsingOptions();

        if ( password != null )
            po.Password = password;


        /*
         * 
         */
        using ( PdfDocument document = PdfDocument.Open( fileName, po ) )
        {
            foreach ( var page in document.GetPages() )
            {
                var text = ContentOrderTextExtractor.GetText( page );

                fc.Pages.Add( new PageContent()
                {
                    Number = page.Number,
                    Text = text,
                } );
            }
        }

        return fc;
    }
}
