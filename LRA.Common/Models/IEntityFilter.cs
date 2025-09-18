using System.Linq.Expressions;

namespace LRA.Common.Models;

public interface IEntityFilter
{
    LambdaExpression GetFilter(Type entityType);
}
