using Riok.Mapperly.Abstractions;

namespace TaskManager.Infrastructure.Mappings;

[Mapper]
public static partial class Mapper
{
    public static partial T Map<K, T>(this K entity);

    public static partial IEnumerable<T> Map<K, T>(this IEnumerable<K> entity);
}
