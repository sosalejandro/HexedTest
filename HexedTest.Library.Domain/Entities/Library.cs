using HexedTest.Common;
using HexedTest.Library.Domain.Enums;
using HexedTest.Library.Domain.Events;
using HexedTest.Library.Domain.Exceptions;
using HexedTest.Library.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Entities;

public class Library : AggregateRoot
{

    public virtual HashSet<Book> Books { get; private set; } = new();
    public virtual HashSet<BorrowOrder> BorrowedBooks { get; private set; } = new();
    public Library()
    {

    }
    public void BorrowBook(string isbn, Guid userId, BorrowOrderPetition petition)
    {
        if (petition is BorrowOrderPetition.BorrowOriginalOrder)
        {
            ApplyDomainEvent(new BorrowOriginalOrder(isbn, userId));
        }
        else
        {
            ApplyDomainEvent(new BorrowCopyOrder(isbn, userId));
        }
    }

    public void ReturnBook(List<string> isbns, Guid userId)
    {
        ApplyDomainEvent(new ReturnBookOrder(isbns, userId));
    }

    protected override void ChangeStateByUsingDomainEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BorrowOriginalOrder e:
                HandleState(e);
                break;
            case BorrowCopyOrder e:
                HandleState(e);
                break;
            case ReturnBookOrder e:
                HandleState(e);
                break;
            default:
                break;
        }
    }

    protected override void ValidateState()
    {
        try
        {
            Parallel.ForEach(Books, book =>
            {
                book.Validate();
            });
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected void HandleState(BorrowOriginalOrder e)
    {
        if (!CheckForOtherBorrowedItems(e))
        {
            var book = Books.Where(b => b.ISBN == e.ISBN)
                .FirstOrDefault();

            if (book is null) throw new ArgumentNullException(nameof(book));

            var originalBorrowed = BorrowOrder.Create(e.UserId, e.ISBN);

            book.SetNewStock(
                BookStock.Create(book.Stock.OriginalAmount - 1,
                book.Stock.CopiesAmount)
                );

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

            book.SetNewStock(
                BookStock.Create(book.Stock.OriginalAmount,
                book.Stock.CopiesAmount - 1)
                );

            copyBorrowed.SetCopy();

            BorrowedBooks.Add(copyBorrowed);
        }
    }

    protected void HandleState(ReturnBookOrder e)
    {
        #region Validate User Input
        if (e.UserId == Guid.Empty) throw new InvalidInputException("Invalid User Id");
        #endregion Validate User Input

        #region CheckUserHasCurrentBorrowedOrders
        bool AreThereBooksThatNeedToBeReturnedByThisUser(BorrowOrder bo) =>
                    bo.UserId == e.UserId &&
                    bo.IsReturned == false;

        var borrowedItems = BorrowedBooks
            .Where(AreThereBooksThatNeedToBeReturnedByThisUser)
            .ToHashSet();

        if (borrowedItems is null || borrowedItems.Count == 0) throw new InvalidLibraryOperationException(
            "There is no book that needs to be returned now by this user.");
        #endregion CheckUserHasCurrentBorrowedOrders

        foreach (string isbn in e.ISBNs)
        {
            #region Validate Input
            if (string.IsNullOrWhiteSpace(isbn)) 
                throw new InvalidInputException("Invalid ISBN input");
            #endregion Validate Input

            #region SearchBorrowOrder
            var borrowedItem = BorrowedBooks
                .Where(x => x.BookISBN == isbn && x.IsReturned == false).FirstOrDefault();
            #endregion SearchBorrowOrder

            if (borrowedItem is not null)
            {
                #region Handle ReturnOrder
                borrowedItem.SetReturned();

                var book = Books.Where(b => b.ISBN == isbn)
                    .FirstOrDefault();

                if (book is null) throw new InvalidLibraryStateException(
                    $"Book {isbn} doesn't exist");

                if (borrowedItem.IsCopy)
                {
                    book.SetNewStock(
                        BookStock.Create(book.Stock.OriginalAmount,
                        book.Stock.CopiesAmount + 1)
                        );
                    continue;
                }

                book.SetNewStock(
                    BookStock.Create(book.Stock.OriginalAmount + 1,
                    book.Stock.CopiesAmount)
                    );

                continue;
                #endregion Handle ReturnOrder
            }

            throw new InvalidLibraryStateException($"Invalid Book ISBN provided [{isbn}]");
        }
    }

    private bool CheckForOtherBorrowedItems(IDomainEvent e)
    {
        Func<BorrowOrder, bool> CopyQuery = (BorrowOrder bo) =>
            bo.IsCopy == true && 
            bo.IsReturned == false &&
            bo.BookISBN == ((BorrowCopyOrder)e).ISBN &&
            bo.UserId == ((BorrowCopyOrder)e).UserId;

        Func<BorrowOrder, bool> OriginalQuery = (BorrowOrder bi) =>
            bi.UserId == ((BorrowOriginalOrder)e).UserId &&
            bi.IsReturned == false &&
            bi.IsCopy == false;

        var query = e switch
        {
            var de when de.GetType() == typeof(BorrowCopyOrder) => CopyQuery,
            _ => OriginalQuery
        };

        var item = BorrowedBooks.Where(query).ToList();

        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        bool LessThanTwoOriginalBooksAtATime = item.Count <= 1 &&
            !item.Exists(i => i.IsCopy);

        bool LessThanOneCopyAtATime = item.Count == 0;

        if (LessThanTwoOriginalBooksAtATime || LessThanOneCopyAtATime)
        {
            return false;
        }

        throw new InvalidLibraryStateException(
            "You cannot borrow more of these items");
    }
}

