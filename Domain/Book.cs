
namespace Sync.Domain;

public class Book
{
    public IList<string> Hashes { get; init; }

    public int? CalibreId { get; private set; }

    public bool Read { get; private set; }

    private bool _needsSync;

    public bool NeedsSync => _needsSync && CalibreId != null;

    public Book(string hash, int? calibreId = null, bool read = false) : this(new[] { hash }, calibreId, read)
    {

    }

    public Book(IEnumerable<string> hashes, int? calibreId = null, bool read = false)
    {
        Hashes = hashes.ToList();
        CalibreId = calibreId;
        Read = read;
    }

    public void MarkAsRead()
    {
        Read = true;
        _needsSync = true;
    }

    public void UpdateFromRemote(Book book)
    {
        CalibreId = book.CalibreId;

        foreach (var hash in book.Hashes.Except(Hashes))
            Hashes.Add(hash);

        if (!book.Read && Read)
            _needsSync = true;
    }

    public bool MatchHash(string hash)
    {
        return Hashes.Contains(hash, StringComparer.InvariantCultureIgnoreCase);
    }

    internal void Synced()
    {
        _needsSync = false;
    }
}