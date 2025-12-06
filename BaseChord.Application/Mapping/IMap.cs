using AutoMapper;

namespace BaseChord.Application.Mapping;

/// <summary>
/// Represents a mapping configuration interface to define object-object mappings
/// for a source type and a destination type using the AutoMapper library.
/// </summary>
/// <typeparam name="TSource">
/// The type of the source object for the mapping.
/// </typeparam>
/// <typeparam name="TDestination">
/// The type of the destination object for the mapping.
/// </typeparam>
public interface IMap<TSource, TDestination>
{
    /// <summary>
    /// Defines a custom mapping configuration between a source type and a destination type
    /// using AutoMapper's <see cref="IMappingExpression{TSource, TDestination}"/>.
    /// </summary>
    /// <param name="mapping">
    /// The mapping expression used to configure how properties from the source type
    /// are mapped to properties in the destination type.
    /// </param>
    public void Mapping(IMappingExpression<TSource, TDestination> mapping);
}
