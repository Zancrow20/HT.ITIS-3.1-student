using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.PermissionChecks.ProfilePermissionsCheck;

public class ProfilePermissionsCheck : IPermissionCheck<IClientRequest>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public ProfilePermissionsCheck(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    public Task<PermissionResult> CheckPermissionAsync(IClientRequest request)
    {
        var context = _contextAccessor.HttpContext;
        var user = context!.User;
        var guidClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (guidClaim == null)
            return Task.FromResult(new PermissionResult(false, "Doesn't have guid!"));
        var guid = Guid.Parse(guidClaim.Value);

        return Task.FromResult(guid == request.Guid ? 
            new PermissionResult(true) : 
            new PermissionResult(false, "Access denied"));
    }
}