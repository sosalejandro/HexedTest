using HexedTest.Common;
using HexedTest.Library.Domain.Events;
using HexedTest.Library.Domain.ValueObjects;

namespace HexedTest.Library.Domain.Entities;

public partial class Library : AggregateRoot
{
    protected void HandleState(BorrowOriginalOrder e)
    {
        if (!CheckForOtherBorrowedItems(e))
        {
            var book = Books.Where(b => b.ISBN == e.ISBN)
                .FirstOrDefault();

            if (book is null) throw new ArgumentNullException(nameof(book));

            var originalBorrowed = BorrowOrder.Create(e.UserId, e.ISBN);

            DecreaseBookOriginalAmount(book);

            BorrowedBooks.Add(originalBorrowed);
        }
    }

    protected void HandleState(BorrowCopyOrder e)
    {
        if (!CheckForOtherBorrowedItems(e))
        {
            var book = Books.Where(b => b.ISBN == e.ISBN)
                .FirstOrDefault();

            if (book is null) throw new ArgumentNullException(nameof(book));

            var copyBorrowed = BorrowOrder.Create(e.UserId, e.ISBN);

            copyBorrowed.SetCopy();

            DecreaseBookCopyAmount(book);

            BorrowedBooks.Add(copyBorrowed);
        }
    }

    internal void DecreaseBookOriginalAmount(Book book)
    {
        book.SetNewStock(
              BookStock.Create(book.Stock.OriginalAmount - 1,
              book.Stock.CopiesAmount)
              );
    }

    internal void DecreaseBookCopyAmount(Book book)
    {

        book.SetNewStock(
            BookStock.Create(book.Stock.OriginalAmount,
            book.Stock.CopiesAmount - 1)
            );
    }
}