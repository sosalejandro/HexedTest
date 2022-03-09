using HexedTest.Library.Api.Commands;

namespace HexedTest.Library.Api.Services
{
    public class LibraryApplicationService
    {
        private readonly IAggregateStore<Domain.Entities.Library> libraryAggregateStore;

        public LibraryApplicationService(IAggregateStore<Domain.Entities.Library> aggregateStore)
        {
            libraryAggregateStore = aggregateStore;
        }

        public async Task HandleAsync(BorrowBookCommand command)
        {
            var library = await libraryAggregateStore.LoadAsync();
            library.BorrowBook(command.ISBN, command.UserId, command.Petition);
            await libraryAggregateStore.SaveAsync(library);
        }

        public async Task HandleAsync(ReturnBookCommand command)
        {
            var library = await libraryAggregateStore.LoadAsync();
            library.ReturnBook(command.ISBNs, command.UserId);
            await libraryAggregateStore.SaveAsync(library);
        }
    }
}
