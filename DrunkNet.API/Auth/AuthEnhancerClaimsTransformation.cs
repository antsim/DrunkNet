using System.Security.Claims;
using DrunkNet.API.Services;
using Microsoft.AspNetCore.Authentication;

namespace DrunkNet.API.Auth;

public class AuthEnhancerClaimsTransformation : IClaimsTransformation
{
    private const string ClaimType = "drunkappUserId";
    
    private readonly IUserService _userService;
    public AuthEnhancerClaimsTransformation(IUserService userService)
    {
        _userService = userService;
    }
    
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userExternalId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        if (string.IsNullOrEmpty(userExternalId))
        {
            return Task.FromResult(principal);
        }
        
        var userId = _userService.GetOrAddByExternalId(userExternalId);
        
        var claimsIdentity = new ClaimsIdentity();
        
        if (!principal.HasClaim(claim => claim.Type == ClaimType))
        {
            claimsIdentity.AddClaim(new Claim(ClaimType, userId.ToString()));
        }

        principal.AddIdentity(claimsIdentity);
        return Task.FromResult(principal);
    }
}