using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Sync.Application.LibrarySync;
using Sync.Application.Syncs.GetProgress;
using Sync.Application.Syncs.UpdateProgress;
using Sync.Application.Users.Auth;
using Sync.Application.Users.CreteUser;

namespace Sync.Application;

public static class Setup
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddHostedService<CalibreWebSync>();

        services.AddSingleton<CreateUserHandler>();
        services.AddSingleton<AuthHandler>();
        services.AddSingleton<GetProgressHandler>();
        services.AddSingleton<UpdateProgressHandler>();
    }

    public static void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/create", [AllowAnonymous] (CreateUserCommand command, CreateUserHandler handler) => handler.Handle(command));
        app.MapGet("/users/auth", [Authorize] (ClaimsPrincipal user, AuthHandler handler) => handler.Handle());
        app.MapPut("/syncs/progress", [Authorize] (ClaimsPrincipal user, UpdateProgressCommand command, UpdateProgressHandler handler) => handler.Handle(user, command));
        app.MapGet("/syncs/progress/{document}", [Authorize] (ClaimsPrincipal user, string document, GetProgressHandler handler) => handler.Handle(document));
    }
}