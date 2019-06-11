using Microsoft.AspNetCore.Identity;

namespace CorsairWebSolutions.IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string TenantId { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
