using HexedTest.Common;
using HexedTest.Library.Domain.Events;
using HexedTest.Library.Domain.Exceptions;
using HexedTest.Library.Domain.ValueObjects;

namespace HexedTest.Library.Domain.Entities;

public partial class Library : AggregateRoot
{
    protected void HandleState(ReturnBookOrder e)
    {
        ValidateInputUserGuid(e.UserId);

        CheckUserHasCurrentBorrowedOrders(BorrowedBooks, e.UserId);

        foreach (string isbn in e.ISBNs)
        {
            ValidateInputISBN(isbn);

            var borrowedItem
                = SearchUnreturnedBorrowOrders(isbn, BorrowedBooks);

            if (borrowedItem is not null)
            {
                HandleReturnOrder(
                    borrowedItem, Books, isbn);

                continue;
            }

            throw new InvalidLibraryStateException(
                $"Invalid Book ISBN provided [{isbn}]");
        }
    }

    internal void CheckUserHasCurrentBorrowedOrders(ICollection<BorrowOrder> borrowedBooks, Guid userId)
    {
        bool AreThereBooksThatNeedToBeReturnedByThisUser(BorrowOrder bo) =>
                    bo.UserId == userId &&
                    bo.IsReturned == false;

        var borrowedItems = borrowedBooks
            .Where(AreThereBooksThatNeedToBeReturnedByThisUser)
            .ToHashSet();

        if (borrowedItems is null || borrowedItems.Count == 0) throw new InvalidLibraryOperationException(
            "There is no book that needs to be returned now by this user.");
    }

    internal void ValidateInputISBN(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new InvalidInputException("Invalid ISBN input");
    }

    internal void ValidateInputUserGuid(Guid guid)
    {
        if (guid == Guid.Empty) throw new InvalidInputException("Invalid User Id");
    }

    internal BorrowOrder? SearchUnreturnedBorrowOrders(string isbn, ICollection<BorrowOrder> borrowedBooks)
    {
        return borrowedBooks
            .Where(x => x.BookISBN == isbn && x.IsReturned == false)
            .FirstOrDefault();
    }

    internal void HandleReturnOrder(BorrowOrder borrowedItem, ICollection<Book> books, string isbn)
    {
        borrowedItem.SetReturned();

        var book = books.Where(b => b.ISBN == isbn).FirstOrDefault();

        if (book is null) throw new InvalidLibraryStateException(
            $"Book {isbn} doesn't exist");

        if (borrowedItem.IsCopy)
        {
            IncreaseBookCopyStock(book);
            return;
        }

        IncreaseBookOriginalStock(book);
    }

    internal void IncreaseBookOriginalStock(Book book)
    {
        book.SetNewStock(
                BookStock.Create(book.Stock.OriginalAmount + 1,
                book.Stock.CopiesAmount)
                );
    }

    internal void IncreaseBookCopyStock(Book book)
    {
        book.SetNewStock(
                BookStock.Create(book.Stock.OriginalAmount,
                book.Stock.CopiesAmount + 1)
                );
    }
}