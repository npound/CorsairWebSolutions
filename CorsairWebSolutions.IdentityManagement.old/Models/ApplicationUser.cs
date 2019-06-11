using Microsoft.AspNetCore.Identity;

namespace CorsairWebSolutions.IdentityManagement.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Tenant { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
