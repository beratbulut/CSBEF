using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSBEF.Models;
using CSBEF.Helpers;
using CSBEF.Interfaces;
using Microsoft.EntityFrameworkCore;
using CSBEF.enums;

namespace CSBEF.Concretes
{
    public class ModularDbContext : DbContext
    {
        public ModularDbContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = GlobalConfiguration.ApiStartOptions.DbContextQueryTrackingBehavior;
            ChangeTracker.LazyLoadingEnabled = GlobalConfiguration.ApiStartOptions.DbContextLazyLoadingEnabled;
            ChangeTracker.AutoDetectChangesEnabled = GlobalConfiguration.ApiStartOptions.DbContextAutoDetectChangesEnabled;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ThrowIfNull();

            List<Type> typeToRegisters = new List<Type>();

            if (GlobalConfiguration.Modules.Any())
            {
                foreach (var module in GlobalConfiguration.Modules)
                {
                    typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
                }
            }

            RegisterEntities(modelBuilder, typeToRegisters);
            RegiserConvention(modelBuilder);
            base.OnModelCreating(modelBuilder);
            RegisterCustomMappings(modelBuilder, typeToRegisters);
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            IEnumerable<Type> entityTypes = null;

            entityTypes = typeToRegisters.Where(x => x.GetTypeInfo().IsSubclassOf(typeof(EntityModelBase)) && !x.GetTypeInfo().IsAbstract);
            if (entityTypes.Any())
            {
                foreach (var type in entityTypes)
                {
                    modelBuilder.Entity(type);
                }
            }
        }

        private static void RegiserConvention(ModelBuilder modelBuilder)
        {
            string[] nameParts;
            string tableName;

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace != null)
                {
                    nameParts = entity.ClrType.Namespace.Split('.');
                    if (nameParts.Length > 2 && nameParts[2].ToLower(GlobalConfiguration.ApiStartOptions.DefaultCultureInfo) == GlobalConfiguration.ApiStartOptions.DefaultMainModuleName)
                    {
                        tableName = entity.ClrType.Name;
                    }
                    else
                    {
                        tableName = string.Concat(nameParts[2], "_", entity.ClrType.Name);
                    }
                    modelBuilder.Entity(entity.Name).ToTable(tableName);
                }
            }
        }

        private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            IEnumerable<Type> customModelBuilderTypes = null;
            IEntityMapper builder = null;

            customModelBuilderTypes = typeToRegisters.Where(x => typeof(IEntityMapper).IsAssignableFrom(x));
            foreach (var builderType in customModelBuilderTypes)
            {
                if (builderType != null && builderType != typeof(IEntityMapper))
                {
                    builder = (IEntityMapper)Activator.CreateInstance(builderType);
                    builder.Mapper(modelBuilder);
                }
            }
        }

        public ContextEntityStates GetEntryState<TEntity>(TEntity entity)
            where TEntity : class, IEntityModelBase
        {
            var entityState = Entry(entity).State;

            if (entityState == EntityState.Detached)
            {
                return ContextEntityStates.Detached;
            }

            if (entityState == EntityState.Unchanged)
            {
                return ContextEntityStates.Unchanged;
            }

            if (entityState == EntityState.Deleted)
            {
                return ContextEntityStates.Deleted;
            }

            if (entityState == EntityState.Modified)
            {
                return ContextEntityStates.Modified;
            }

            if (entityState == EntityState.Added)
            {
                return ContextEntityStates.Added;
            }

            return ContextEntityStates.Unknown;
        }
    }
}