using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HospitalApi.Models;

namespace HospitalApi.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _roles;

        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // For now, we'll implement a simple role check
            // In a real application, you'd get the user from JWT token or session
            var userRole = GetUserRoleFromContext(context);
            
            if (userRole == null || !_roles.Contains(userRole.Value))
            {
                context.Result = new ForbidResult();
            }
        }

        private UserRole? GetUserRoleFromContext(AuthorizationFilterContext context)
        {
            // This is a simplified implementation
            // In a real app, you'd extract this from JWT token or session
            var userRoleHeader = context.HttpContext.Request.Headers["X-User-Role"].FirstOrDefault();
            
            if (Enum.TryParse<UserRole>(userRoleHeader, out var role))
            {
                return role;
            }

            // For testing purposes, allow access if no role header is present
            // In production, this should return null to deny access
            return UserRole.Admin;
        }
    }
} 