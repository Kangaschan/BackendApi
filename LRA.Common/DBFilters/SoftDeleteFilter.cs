using System.Linq.Expressions;
using LRA.Common.Models;

namespace LRA.Common.DBFilters;

public class SoftDeleteFilter : IEntityFilter
{
    public LambdaExpression GetFilter(Type entityType)
    {
        if (!typeof(ISoftDeletableEntity).IsAssignableFrom(entityType))
            return null;

        var param = Expression.Parameter(entityType, "e");
        var prop = Expression.Property(param, nameof(ISoftDeletableEntity.IsDeleted));
        var constant = Expression.Constant(false);
        var body = Expression.Equal(prop, constant);
        return Expression.Lambda(body, param);
    }
}
