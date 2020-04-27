using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KnowledgeSpace.WebPortal.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetFullName(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity)claimsPrincipal.Identity)
                .Claims
                .SingleOrDefault(x => x.Type == "fullName");
            return claim.Value;
        }
    }
}