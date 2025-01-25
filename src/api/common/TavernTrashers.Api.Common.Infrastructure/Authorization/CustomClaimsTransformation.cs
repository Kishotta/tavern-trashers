using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using TavernTrashers.Api.Common.Application.Authorization;
using TavernTrashers.Api.Common.Application.Exceptions;
using TavernTrashers.Api.Common.Infrastructure.Authentication;

namespace TavernTrashers.Api.Common.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation(IServiceScopeFactory serviceScopeFactory)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(claim => claim.Type == CustomClaims.Sub)) return principal;

        using var scope = serviceScopeFactory.CreateScope();
        
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        var identityId = principal.GetIdentityId();
        
        var result = await permissionService.GetUserPermissionsAsync(identityId);
        if (result.IsFailure)
            throw new TavernTrashersException(nameof(IPermissionService.GetUserPermissionsAsync), result.Error);

        var claimsIdentity = new ClaimsIdentity();
        
        claimsIdentity.AddClaim(new Claim(CustomClaims.Sub, result.Value.UserId.ToString()));
        
        foreach(var permission in result.Value.Permissions)
            claimsIdentity.AddClaim(new Claim(CustomClaims.Permission, permission));
        
        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}