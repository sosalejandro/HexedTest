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

