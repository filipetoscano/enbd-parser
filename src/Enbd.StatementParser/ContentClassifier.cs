namespace Enbd.StatementParser;

/// <summary />
public class ContentClassifier
{
    /// <summary />
    public Statement? Classify( FileContent content )
    {
        var firstPage = content.Pages[ 0 ];

        if ( firstPage.Text.Contains( "Credit Card Statement" ) == true )
            return Statement.CreditCard;

        if ( firstPage.Text.Contains( "CURRENT ACCOUNT" ) == true )
            return Statement.CurrentAccount;

        return null;
    }
}
