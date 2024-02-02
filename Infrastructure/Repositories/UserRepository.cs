using Sync.Domain;
using Sync.Domain.Repositories;

namespace Sync.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private IList<User> _users = new List<User>();

    public void Add(User user)
    {
        lock (_users)
        {
            if (Get(user.HostUrl, user.Username) != null)
                throw new ArgumentOutOfRangeException(nameof(user), "User already exists");
            _users.Add(user);
        }
    }

    public User? Get(string hostUrl, string username)
    {
        return _users.SingleOrDefault(u => u.HostUrl == hostUrl && u.Username == username);
    }

    public void Update(User user)
    {
        lock (_users)
        {
            var existing = Get(user.HostUrl, user.Username);
            if (existing == null)
                throw new ArgumentOutOfRangeException(nameof(user), "Could not find user");
            _users.Remove(existing);
            _users.Add(user);
        }
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }
}