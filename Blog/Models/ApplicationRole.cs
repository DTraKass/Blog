using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
