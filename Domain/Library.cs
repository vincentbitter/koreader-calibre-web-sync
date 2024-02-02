

namespace Sync.Domain;

public class Library
{
    public DateTimeOffset? LastSync { get; private set; }
    private IList<Book> _books = new List<Book>();

    public void Add(Book book)
    {
        _books.Add(book);
    }

    public Book? GetBookByHash(string hash)
    {
        return _books.FirstOrDefault(b => b.MatchHash(hash));
    }

    public Book? GetBookByHash(IEnumerable<string> hashes)
    {
        return _books.FirstOrDefault(b => hashes.Any(b.MatchHash));
    }

    public void MarkBookAsRead(string document)
    {
        var book = GetBookByHash(document);
        if (book == null)
            Add(new Book(document, null, true));
        else
            book.MarkAsRead();
    }

    public bool NeedsSync()
    {
        return (NotUpdatedLastHour && HasUnknownBooks) || HasUnsyncedBooks;
    }

    private bool NotUpdatedLastHour => LastSync == null || LastSync < DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(-1));
    private bool HasUnknownBooks => _books.Any(b => b.CalibreId == null);
    private bool HasUnsyncedBooks => _books.Any(b => b.NeedsSync);

    public IEnumerable<Book> GetBookToSync()
    {
        return _books.Where(b => b.NeedsSync).ToList();
    }

    public void MarkAsSynced()
    {
        LastSync = DateTimeOffset.UtcNow;
    }
}