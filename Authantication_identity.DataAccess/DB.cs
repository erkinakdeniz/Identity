using Microsoft.AspNet.Identity.EntityFramework;

namespace Authantication_identity.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Authantication_identity.Entities.IdentityModels;

    public partial class DB : IdentityDbContext<User>
    {
        public DB()
            : base("name=DB")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
