using Riok.Mapperly.Abstractions;

namespace TaskManager.Infrastructure.Mappings;

[Mapper]
internal static partial class Mapper
{
    internal static partial TResult Map<TSource, TResult>(this TSource source);

    internal static partial IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source);
}
