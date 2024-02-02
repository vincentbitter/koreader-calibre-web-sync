namespace Sync.Domain.Repositories;

public interface IUserRepository
{
    void Add(User user);
    User? Get(string hostUrl, string user);
    void Update(User user);
    IEnumerable<User> GetAll();
}