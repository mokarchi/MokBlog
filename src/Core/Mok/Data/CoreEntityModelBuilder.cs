﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mok.Membership;
using MokCore.Data;

namespace Mok.Data
{
    /// <summary>
    /// The core entity model.
    /// </summary>
    public class CoreEntityModelBuilder : IEntityModelBuilder
    {
        public void CreateModel(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("Core_User");
            builder.Entity<Role>().ToTable("Core_Role");
            builder.Entity<IdentityUserClaim<int>>().ToTable("Core_UserClaim");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("Core_RoleClaim");
            builder.Entity<IdentityUserRole<int>>().ToTable("Core_UserRole");
            builder.Entity<IdentityUserLogin<int>>().ToTable("Core_UserLogin");
            builder.Entity<IdentityUserToken<int>>().ToTable("Core_UserToken");
            builder.Entity<Meta>(entity =>
            {
                entity.ToTable("Core_Meta");
                entity.HasKey(e => e.Id).IsClustered(clustered: false);
                entity.HasIndex(e => new { e.Type, e.Key }).IsUnique().IsClustered();
            });
        }
    }
}
