using Odp.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odp.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        public DbSet<Odp.Models.ApplicationUser> ApplicationUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /*#region "Seed data"
            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "Admin", NormalisedName = "ADMIN" },
                new { Id = "2", Name = "Customer", NormalisedName = "CUSTOMER" },
                new { Id = "3", Name = "Author", NormalisedName = "AUTHOR" },
                new { Id = "4", Name = "Moderator", NormalisedName = "MODERATOR" }
            );
            #endregion*/
        }
    }
}
