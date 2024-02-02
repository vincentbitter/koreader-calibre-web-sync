
using Sync.Domain;
using Sync.Domain.Repositories;
using Sync.Infrastructure.Services;

namespace Sync.Application.LibrarySync;

class CalibreWebSync : BackgroundService
{
    private readonly IUserRepository _userRepository;
    private readonly CalibreWebService _calibre;

    public CalibreWebSync(IUserRepository userRepository, CalibreWebService calibre)
    {
        _userRepository = userRepository;
        _calibre = calibre;
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await SyncBooks(cancellationToken);
            await Task.Delay(1000, cancellationToken);
        }
    }

    private async Task SyncBooks(CancellationToken cancellationToken)
    {
        var users = _userRepository.GetAll().Where(u => u.Library.NeedsSync()).ToList();
        foreach (var user in users)
        {
            await SyncBooks(user, cancellationToken);
        }
    }

    private async Task SyncBooks(User user, CancellationToken cancellationToken)
    {
        var library = user.Library;
        library.MarkAsSynced();
        await UpdateBooksFromRemote(user, library, cancellationToken);
        foreach (var book in library.GetBookToSync())
        {
            if (!book.CalibreId.HasValue)
                throw new ApplicationException("Calibre ID should never be null at this stage");
            try
            {
                await _calibre.MarkBookAsReadSync(user, book.CalibreId.Value, cancellationToken);
                book.Synced();
            }
            catch
            {
                // Ignore exceptions, as there is no user interface to report issues
                // todo: add logging
            }
        }
    }

    private async Task UpdateBooksFromRemote(User user, Library library, CancellationToken cancellationToken)
    {
        var booksInCalibre = await _calibre.GetBooksAsync(user, cancellationToken);
        foreach (var book in booksInCalibre)
        {
            var existing = library.GetBookByHash(book.Hashes);
            if (existing != null)
                existing.UpdateFromRemote(book);
            else
                library.Add(book);
        }
    }
}