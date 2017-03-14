using Blog.Turnmeup.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Turnmeup.DAL.Contexts
{
        public class IdentityDataContext : IdentityDbContext<AppUser>
        {

            public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
            {
            }
        }
}
