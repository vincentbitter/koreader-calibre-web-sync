using System.Security.Claims;
using Sync.Domain;
using Sync.Domain.Repositories;

namespace Sync.Application.Syncs.UpdateProgress;

class UpdateProgressHandler
{
    private readonly IUserRepository _userRepository;

    public UpdateProgressHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public UpdateProgressResponse Handle(ClaimsPrincipal principal, UpdateProgressCommand command)
    {
        if (command.Percentage == 1)
        {
            var library = FindLibrary(principal);
            library.MarkBookAsRead(command.Document);
        }
        return new UpdateProgressResponse(command.Document, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    private Library FindLibrary(ClaimsPrincipal principal)
    {
        var userDetails = principal.Claims.Single(c => c.Type == ClaimTypes.Name);
        var library = _userRepository.Get(userDetails.Issuer, userDetails.Value)?.Library
            ?? throw new ApplicationException("Library not found");
        return library;
    }
}