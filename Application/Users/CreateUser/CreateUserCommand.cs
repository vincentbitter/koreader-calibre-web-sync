namespace Sync.Application.Users.CreteUser;

record CreateUserCommand(
    string Username,
    string Password
);