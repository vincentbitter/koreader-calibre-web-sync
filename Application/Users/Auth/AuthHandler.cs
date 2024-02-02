namespace Sync.Application.Users.Auth;

class AuthHandler
{
    public IResult Handle()
    {
        // Authentication is handled by middleware, 
        // so if this handler is reached, everything is fine
        return Results.Ok();
    }
}