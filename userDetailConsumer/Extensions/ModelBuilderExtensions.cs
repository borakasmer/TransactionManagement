using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace userDetailConsumer.DB
{
//https://github.com/bayramucuncu/B3.Extensions.Data
public static class ModelBuilderExtensions
    {
        private static readonly MethodInfo SetSoftDeleteFilterMethod = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == nameof(SetSoftDeleteFilter));

        public static void SetSoftDeleteFilter(this ModelBuilder modelBuilder, Type entityType)
        {
            SetSoftDeleteFilterMethod.MakeGenericMethod(entityType)
                .Invoke(null, new object[] { modelBuilder });
        }

        public static void SetSoftDeleteFilter<TEntity>(this ModelBuilder modelBuilder)
            where TEntity : class
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => !EF.Property<bool>(x, "IsDeleted"));
        }

        public static void UseSoftDelete<TSoftDelete>(this ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes().Where(t => typeof(TSoftDelete).IsAssignableFrom(t.ClrType)))
                modelBuilder.SetSoftDeleteFilter(type.ClrType);
        }
    }
}