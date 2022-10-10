using System.Security.Claims;

namespace DrunkNet.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetInternalUserId(this ClaimsPrincipal principal)
    {
        var internalUserIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "drunkappUserId");

        if (internalUserIdClaim == null)
        {
            return 0;
        }
        
        return Int32.TryParse(internalUserIdClaim.Value, out var internalUserId) ? internalUserId : 0;
    }
}