using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mok.Helpers;
using MokCore.Data;
using System;
using System.Diagnostics;

namespace Mok.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> logger;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory loggerFactory) 
            : base(options)
        {
            logger = loggerFactory.CreateLogger<ApplicationDbContext>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // find entities and model builders from app assemblies
            var entityTypes = TypeFinder.Find<Entity>();
            var modelBuilderTypes = TypeFinder.Find<IEntityModelBuilder>();

            // add entity types to the model
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
                logger.LogDebug($"Entity: '{type.Name}' added to model");
            }

            base.OnModelCreating(modelBuilder);

            // add mappings and relations
            foreach (var builderType in modelBuilderTypes)
            {
                if (builderType != null && builderType != typeof(IEntityModelBuilder))
                {
                    logger.LogDebug($"ModelBuilder '{builderType.Name}' added to model");
                    var builder = (IEntityModelBuilder)Activator.CreateInstance(builderType);
                    builder.CreateModel(modelBuilder);
                }
            }
        }
    }
}
