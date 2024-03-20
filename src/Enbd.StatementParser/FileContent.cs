namespace Enbd.StatementParser;


/// <summary />
public class FileContent
{
    /// <summary />
    public string Filename { get; set; } = default!;

    /// <summary />
    public List<PageContent> Pages { get; set; } = new List<PageContent>();
}


/// <summary />
public class PageContent
{
    /// <summary />
    public int Number { get; set; }

    /// <summary />
    public string Text { get; set; } = default!;
}