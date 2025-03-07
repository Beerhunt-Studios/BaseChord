using AutoMapper;

namespace BaseChord.Application.Mapping;

public interface IMap<TSource, TDestination>
{
    public void Mapping(IMappingExpression<TSource, TDestination> mapping);
}
