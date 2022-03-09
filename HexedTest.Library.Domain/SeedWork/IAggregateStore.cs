
namespace HexedTest.Library.Domain.SeedWork;
public interface IAggregateStore<T> where T : class
{
    Task SaveAsync(T hotel);
    Task<T> LoadAsync(Guid hotel);
}
