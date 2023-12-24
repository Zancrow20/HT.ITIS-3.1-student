using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.UserManagement.PermissionChecks.AdminPermissionCheck;

public class AdminPermissionCheck : IPermissionCheck<IAdminRequest>
{
    private readonly HttpContext? _context;

    public AdminPermissionCheck(IHttpContextAccessor contextAccessor)
    {
        _context = contextAccessor.HttpContext;
    }

    public Task<PermissionResult> CheckPermissionAsync(IAdminRequest request)
    {
        var user = _context!.User;
        var isAdminRole = user.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == Roles.Admin.ToString()) != null;
        var message = isAdminRole ? null : "User doesn't have admin permissions";
        return Task.FromResult(new PermissionResult(isAdminRole, message));
    }
}