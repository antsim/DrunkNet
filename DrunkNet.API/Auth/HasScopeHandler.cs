using Microsoft.AspNetCore.Authorization;

namespace DrunkNet.API.Auth
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        private readonly ILogger _logger;
        public HasScopeHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("DrunkNet.API.Auth.HasScopeHandler");
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            var scopes = context.User
                .FindAll(c => c.Type == "permissions" && c.Issuer == requirement.Issuer)
                .Select(c => c.Value);
            
            if (scopes.Any(s => s == requirement.Scope))
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}
