using Riok.Mapperly.Abstractions;

namespace TaskManager.Infrastructure.Mappings;

[Mapper]
public static partial class Mapper
{
    public static partial TResult Map<TSource, TResult>(this TSource entity);

    public static partial IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> entity);
}
