using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Core.Concretes {
    public class ModularDbContext : DbContext {
        public ModularDbContext (DbContextOptions options) : base (options) {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            #region Variables

            List<Type> typeToRegisters = new List<Type> ();

            #endregion Variables

            #region Action Body

            foreach (var module in GlobalConfiguration.Modules) {
                typeToRegisters.AddRange (module.Assembly.DefinedTypes.Select (t => t.AsType ()));
            }

            RegisterEntities (modelBuilder, typeToRegisters);
            RegiserConvention (modelBuilder);
            base.OnModelCreating (modelBuilder);
            RegisterCustomMappings (modelBuilder, typeToRegisters);

            #endregion Action Body

            #region Clear Memory

            modelBuilder = null;
            typeToRegisters = null;

            #endregion Clear Memory
        }

        private static void RegiserConvention (ModelBuilder modelBuilder) {
            #region Variables

            string[] nameParts;
            string tableName;

            #endregion Variables

            #region Action Body

            foreach (var entity in modelBuilder.Model.GetEntityTypes ()) {
                if (entity.ClrType.Namespace != null) {
                    nameParts = entity.ClrType.Namespace.Split ('.');
                    tableName = string.Concat (nameParts[2], "_", entity.ClrType.Name);
                    modelBuilder.Entity (entity.Name).ToTable (tableName);
                }
            }

            #endregion Action Body
        }

        private static void RegisterEntities (ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters) {
            #region Variables

            IEnumerable<Type> entityTypes = null;

            #endregion Variables

            #region Action Body

            entityTypes = typeToRegisters.Where (x => x.GetTypeInfo ().IsSubclassOf (typeof (EntityModelBase)) && !x.GetTypeInfo ().IsAbstract);
            foreach (var type in entityTypes) {
                modelBuilder.Entity (type);
            }

            #endregion Action Body

            #region Clear Memory

            modelBuilder = null;
            typeToRegisters = null;
            entityTypes = null;

            #endregion Clear Memory
        }

        private static void RegisterCustomMappings (ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters) {
            #region Variables

            IEnumerable<Type> customModelBuilderTypes = null;
            ICustomModelBuilder builder = null;

            #endregion Variables

            #region Action Body

            customModelBuilderTypes = typeToRegisters.Where (x => typeof (ICustomModelBuilder).IsAssignableFrom (x));
            foreach (var builderType in customModelBuilderTypes) {
                if (builderType != null && builderType != typeof (ICustomModelBuilder)) {
                    builder = (ICustomModelBuilder) Activator.CreateInstance (builderType);
                    builder.Build (modelBuilder);
                }
            }

            #endregion Action Body

            #region Clear Memory

            modelBuilder = null;
            typeToRegisters = null;
            customModelBuilderTypes = null;
            builder = null;

            #endregion Clear Memory
        }
    }
}