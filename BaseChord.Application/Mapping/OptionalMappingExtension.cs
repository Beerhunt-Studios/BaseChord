using BaseChord.Application.Models;

namespace BaseChord.Application.Mapping;

using AutoMapper;
using System.Linq.Expressions;

/// <summary>
/// Extension methods for mapping Optional&lt;T&gt; types using AutoMapper.
/// </summary>
public static class AutoMapperOptionalExtensions
{
    /// <summary>
    /// Maps the value from an Optional&lt;T&gt; source property.
    /// If the source property has no value (HasValue == false), it will not override the destination value.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <typeparam name="TMember">Type of the member/property being mapped.</typeparam>
    /// <param name="opt">The member configuration expression.</param>
    /// <param name="sourceMember">The source property expression returning an Optional&lt;T&gt;.</param>
    /// <returns>The updated configuration expression.</returns>
    public static IMemberConfigurationExpression<TSource, TDestination, TMember?>
        MapFromOptional<TSource, TDestination, TMember>(
            this IMemberConfigurationExpression<TSource, TDestination, TMember?> opt,
            Expression<Func<TSource, Optional<TMember>>> sourceMember)
    {
        opt.MapFrom((src, dest, destMember, context) =>
        {
            var optional = sourceMember.Compile().Invoke(src);

            return optional.HasValue ? optional.Value : destMember;
        });
        return opt;
    }
}
