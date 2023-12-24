using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.PermissionChecks.ProfilePermissionsCheck;

public class ProfilePermissionsCheck : IPermissionCheck<IClientRequest>
{
    private readonly HttpContext? _context;

    public ProfilePermissionsCheck(IHttpContextAccessor contextAccessor)
    {
        _context = contextAccessor.HttpContext;
    }
    
    public Task<PermissionResult> CheckPermissionAsync(IClientRequest request)
    {
        var user = _context!.User;
        var guidClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (guidClaim == null)
            return Task.FromResult(new PermissionResult(false, "Doesn't have guid!"));
        var guid = Guid.Parse(guidClaim.Value);

        return guid == request.Guid ? 
            Task.FromResult(new PermissionResult(true)) : 
            Task.FromResult(new PermissionResult(false, "Access denied"));
    }
}