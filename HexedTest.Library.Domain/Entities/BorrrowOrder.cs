using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Entities;

public class BorrowOrder
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string BookISBN { get; init; }
    public bool IsCopy { get; private set; } = false;
    public bool IsReturned { get; private set; } = false;
    public DateTime DateRequested { get; init; }
    public DateTime? DateReturned { get; private set; }

    internal BorrowOrder(Guid id, Guid userId, string bookISBN)
    {
        Id = id;
        UserId = userId;
        BookISBN = bookISBN;
        DateRequested = DateTime.Now;
    }

    public static BorrowOrder Create(Guid userId, string bookISBN)
    {
        return new BorrowOrder(Guid.NewGuid(), userId, bookISBN);
    }

    public void SetCopy()
    {
        IsCopy = true;
    }

    public void SetReturned()
    {
        IsReturned = true;
        DateReturned = DateTime.Now;
    }
}

