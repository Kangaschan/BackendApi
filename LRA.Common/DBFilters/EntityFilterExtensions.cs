using LRA.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LRA.Common.DBFilters;

public static class EntityFilterExtensions
{
    public static ModelBuilder ApplyGlobalFilters(this ModelBuilder modelBuilder, params IEntityFilter[] filters)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var filter in filters)
            {
                var filterExpression = filter.GetFilter(entityType.ClrType);
                if (filterExpression != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(filterExpression);
                }
            }
        }
        return modelBuilder;
    }

    public static void HandleSoftDelete(this DbContext context)
    {
        var entries = context.ChangeTracker.Entries<ISoftDeletableEntity>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }
}
