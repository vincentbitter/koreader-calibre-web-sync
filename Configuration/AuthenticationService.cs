using Sync.Domain;
using Sync.Domain.Repositories;
using Sync.Infrastructure.Services;

namespace Sync.Configuration;

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly CalibreWebService _calibre;

    public AuthenticationService(IUserRepository userRepository, CalibreWebService calibre)
    {
        _userRepository = userRepository;
        _calibre = calibre;
    }

    public async Task<bool> AuthenticateAsync(User user)
    {
        var cachedUser = _userRepository.Get(user.HostUrl, user.Username);
        if (cachedUser != null && cachedUser.Password == user.Password)
            return true;

        if (await _calibre.LoginAsync(user))
        {
            if (cachedUser != null)
            {
                _userRepository.Update(user);
            }
            else
            {
                _userRepository.Add(user);
            }
            return true;
        }

        return false;
    }


}