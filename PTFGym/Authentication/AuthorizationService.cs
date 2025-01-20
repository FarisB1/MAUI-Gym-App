using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTFGym.Authentication
{
    public class AuthorizationService
    {
        private readonly AuthenticationService _authenticationService;

        public AuthorizationService(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<bool> IsUserInRoleAsync(string requiredRole)
        {
            var role = await _authenticationService.GetUserRoleAsync();
            return role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase);
        }
    }

}
