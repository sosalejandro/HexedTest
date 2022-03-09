
namespace HexedTest.Library.Domain.SeedWork;
public interface IAggregateStore<T> where T : class
{
    Task SaveAsync(T entity);
    Task<T> LoadAsync();
}
