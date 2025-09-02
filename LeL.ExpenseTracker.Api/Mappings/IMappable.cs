using AutoMapper;

namespace LeL.ExpenseTracker.Api.Mappings;

public interface IMappable<TSource>
{
    void CreateMap(Profile profile) => profile.CreateMap(typeof(TSource), GetType());
}