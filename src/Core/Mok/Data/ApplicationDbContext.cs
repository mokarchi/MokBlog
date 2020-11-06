﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Mok.Helpers;
using MokCore.Data;
using System;
using System.Linq;

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

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }

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
