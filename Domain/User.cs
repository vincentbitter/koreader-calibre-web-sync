namespace Sync.Domain;

public record User(string HostUrl, string Username, string Password)
{
    public Library Library { get; } = new Library();
}