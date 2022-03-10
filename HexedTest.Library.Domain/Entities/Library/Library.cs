using HexedTest.Common;
using HexedTest.Library.Domain.Enums;
using HexedTest.Library.Domain.Events;
using HexedTest.Library.Domain.Exceptions;

namespace HexedTest.Library.Domain.Entities;

public partial class Library : AggregateRoot
{

    public virtual HashSet<Book> Books { get; private set; } = new();
    public virtual HashSet<BorrowOrder> BorrowedBooks { get; private set; } = new();
    public Library()
    {

    }

    internal Library(HashSet<Book> books)
    {
        Books = books;
    }

    public static Library CreateFromBooks(HashSet<Book> books)
    {
        if (books is null || books.Count == 0) 
            throw new InvalidLibraryOperationException(
                "Cannot add books to the library from an empty collection");

        return new Library(books);
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

