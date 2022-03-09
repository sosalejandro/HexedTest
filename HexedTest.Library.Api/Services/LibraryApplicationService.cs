

namespace HexedTest.Library.Api.Services
{
    public class LibraryApplicationService
    {
        private readonly IAggregateStore<Domain.Entities.Library> libraryAggregateStore;

        public LibraryApplicationService(IAggregateStore<Domain.Entities.Library> aggregateStore)
        {
            libraryAggregateStore = aggregateStore;
        }
    }
}
