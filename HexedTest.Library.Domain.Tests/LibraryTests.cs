﻿// <auto-generated/>
using HexedTest.Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Tests;

[Collection("Library")]
public class LibraryTests
{
    [Fact]
    public void BorrowBook_Should_BorrowOriginalBook_AndOnlyDecreaseOriginalStockAmount_WhenThereIsStock()
    {
        // Arrange 
        BorrowOrder order;
        int expectedCount = 1;
        int originalAmount = 20;
        int copiesAmount = 50;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-2-8340-6328-4";

        BorrowOrder borrowOrder = BorrowOrder.Create(userId, bookISBN);

        var library = new Entities.Library()
        {
            Books = { CreateBaseBook(bookISBN, original: originalAmount, copies: copiesAmount) }
        };

        // Act
        library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowOriginalOrder);
        order = library.BorrowedBooks.FirstOrDefault(bb => bb.BookISBN == borrowOrder.BookISBN);

        // Assert
        Assert.True(library.BorrowedBooks.Count == expectedCount,
            $"Count [{library.BorrowedBooks.Count}] doens't match [{expectedCount}]");

        Assert.NotNull(order);

        Assert.All(library.Books,
            b => Assert.Equal(originalAmount - 1, b.Stock.OriginalAmount));

        Assert.All(library.Books,
            b => Assert.Equal(copiesAmount, b.Stock.CopiesAmount));

        Assert.Equal(borrowOrder.BookISBN, order.BookISBN);

        Assert.False(order.IsCopy,
            "Order shouldn't be copy or null or field set to null");

        Assert.False(order.IsReturned,
            "Book shouldn't be returned or field set to null");
    }

    [Fact]
    public void BorrowBook_Should_BorrowCopyBook_AndOnlyDecreaseCopiesStockAmount_WhenThereIsStock()
    {
        // Arrange 
        BorrowOrder order;
        int expectedCount = 1;
        int originalAmount = 50;
        int copiesAmount = 20;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-2-8340-6328-4";

        BorrowOrder borrowOrder = BorrowOrder.Create(userId, bookISBN);

        var library = new Entities.Library()
        {
            Books = { CreateBaseBook(bookISBN, original: originalAmount, copies: copiesAmount) }
        };

        // Act
        library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowCopyOrder);
        order = library.BorrowedBooks.FirstOrDefault(bb => bb.BookISBN == borrowOrder.BookISBN);

        // Assert
        Assert.True(library.BorrowedBooks.Count == expectedCount,
            $"Count [{library.BorrowedBooks.Count}] doens't match [{expectedCount}]");

        Assert.NotNull(order);

        Assert.All(library.Books,
            b => Assert.Equal(copiesAmount - 1, b.Stock.CopiesAmount));

        Assert.All(library.Books,
            b => Assert.Equal(originalAmount, b.Stock.OriginalAmount));

        Assert.Equal(borrowOrder.BookISBN, order.BookISBN);

        Assert.True(order.IsCopy,
            "Order shouldn't be original or null or field set to null");

        Assert.False(order.IsReturned,
            "Book shouldn't be returned or field set to null");
    }

    [Fact]
    public void BorrowBook_BorrowOriginalBook_Should_Throw_WhenThereIsNoStock()
    {
        // Arrange
        BorrowOrder order;
        int expectedCount = 0;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-2-8340-6328-4";
        BorrowOrder borrowOrder = BorrowOrder.Create(userId, bookISBN);

        var library = new Entities.Library()
        {
            Books = { CreateBaseBook(bookISBN, 0, 15) }
        };

        Action<Entities.Library> BorrowOriginal = (Entities.Library library) => library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowOriginalOrder);

        Func<Entities.Library, BorrowOrder> SearchBorroweOrder = (Entities.Library library) => library.BorrowedBooks
        .FirstOrDefault(bo => bo.BookISBN == borrowOrder.BookISBN);

        // Act & Assert
        Assert.Throws<OutOfStockException>(() => BorrowOriginal(library));
        order = SearchBorroweOrder(library);

        Assert.True(library.BorrowedBooks.Count == expectedCount,
            "Count [{library.BorrowedBooks.Count}] doens't match [{expectedCount}]");
        Assert.Null(order);

    }

    [Fact]
    public void BorrowBook_BorrowCopyBook_Should_Throw_WhenThereIsNoStock()
    {
        // Arrange
        BorrowOrder order;
        int expectedCount = 0;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-2-8340-6328-4";
        BorrowOrder borrowOrder = BorrowOrder.Create(userId, bookISBN);

        var library = new Entities.Library()
        {
            Books = { CreateBaseBook(bookISBN, 15, 0) }
        };

        Action<Entities.Library> BorrowCopy = (Entities.Library library) => library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowCopyOrder);

        Func<Entities.Library, BorrowOrder> SearchBorroweOrder = (Entities.Library library) => library.BorrowedBooks
        .FirstOrDefault(bo => bo.BookISBN == borrowOrder.BookISBN);

        // Act & Assert
        Assert.Throws<OutOfStockException>(() => BorrowCopy(library));
        order = SearchBorroweOrder(library);

        Assert.True(library.BorrowedBooks.Count == expectedCount,
            "Count [{library.BorrowedBooks.Count}] doens't match [{expectedCount}]");
        Assert.Null(order);

    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void BorrowBook_Should_BorrowOriginalBook_WhenTheSameUserHaveNotHaveBorrowedMoreThanTwoBooks(int amountToBorrow)
    {
        // Arrange
        int expectedCount = amountToBorrow + 1;
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        Action<Entities.Library, string> BorrowOriginal =
            (Entities.Library lib, string bookISBN) =>
            lib.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowOriginalOrder);

        Queue<Action> borrowQueue = new Queue<Action>(2);

        for (int i = 0; i <= amountToBorrow; i++)
        {
            borrowQueue.Enqueue(
                () => BorrowOriginal(library, bookISBNs[i])
                );
        }

        // Act
        foreach (var borrowAction in borrowQueue)
        {
            borrowAction();
        }

        var currentBorrowedBooksByTheSameUser =
            library.BorrowedBooks.Where(bo =>
            bo.IsReturned == false &&
            bo.IsCopy == false &&
            bo.UserId == userId);

        // Assert
        Assert.Equal(expectedCount, currentBorrowedBooksByTheSameUser.Count());
        Assert.All(currentBorrowedBooksByTheSameUser,
            bo => Assert.True(bo.IsReturned == false && bo.IsCopy == false && bo.UserId == userId,
            "BorrowOrder is either returned, a copy or doesn't belong to this user")
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void BorrowBook_Should_BorrowOriginalBook_WhenTheSameUserHaveNotHaveBorrowedMoreThanTwoBooks_AndHasDifferentCopiesUnreturned(int amountToBorrow)
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        int expectedCount = amountToBorrow + 1;

        Func<IEnumerable<BorrowOrder>, IEnumerable<BorrowOrder>, int> expectedBorrowOrders
            = (original, copies) => original.Count() + copies.Count();

        Action<Entities.Library, string, BorrowOrderPetition> BorrowBook =
            (Entities.Library lib, string bookISBN, BorrowOrderPetition petition) =>
            lib.BorrowBook(bookISBN, userId, petition);


        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        int randomCopies = Random.Shared.Next(3);

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs.ElementAt(0)),
                CreateBaseBook(bookISBNs.ElementAt(1)),
                CreateBaseBook(bookISBNs.ElementAt(2)),
                CreateBaseBook(bookISBNs.ElementAt(3))
            }
        };

        // Before using explicit index, Enqueued action was memoized and it was using the latest index to the moment the for loop did stop.
        // Generating a bug which the most of the time had a value of index number 2 leading to test failure due to stock limit exceeded on the same item.
        Queue<Action> borrowQueue = new Queue<Action>(10);


        for (int i = 0; i <= amountToBorrow; i++)
        {
            if (i == 0) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(0), BorrowOrderPetition.BorrowOriginalOrder)
                );

            if (i == 1) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(1), BorrowOrderPetition.BorrowOriginalOrder)
                );

            if (i == 2) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(2), BorrowOrderPetition.BorrowOriginalOrder)
                );

            if (i == 3) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(3), BorrowOrderPetition.BorrowOriginalOrder)
                );
        }

        for (int i = 0; i <= randomCopies; i++)
        {
            if (i == 0) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(0), BorrowOrderPetition.BorrowCopyOrder)
                );

            if (i == 1) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(1), BorrowOrderPetition.BorrowCopyOrder)
                );

            if (i == 2) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(2), BorrowOrderPetition.BorrowCopyOrder)
                );

            if (i == 3) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs.ElementAt(3), BorrowOrderPetition.BorrowCopyOrder)
                );
        }

        // Act
        foreach (var borrowAction in borrowQueue)
        {
            borrowAction();
        }

        var currentBorrowedOriginalBooksByTheSameUser =
            library.BorrowedBooks.Where(bo =>
            bo.IsReturned == false &&
            bo.IsCopy == false &&
            bo.UserId == userId);

        var currentBorrowedCopylBooksByTheSameUser =
            library.BorrowedBooks.Where(bo =>
            bo.IsReturned == false &&
            bo.IsCopy == true &&
            bo.UserId == userId);

        // Assert
        Assert.Equal(expectedCount, currentBorrowedOriginalBooksByTheSameUser.Count());

        Assert.All(currentBorrowedOriginalBooksByTheSameUser,
            bo => Assert.True(bo.IsReturned == false && bo.IsCopy == false && bo.UserId == userId,
            "BorrowOrder is either returned, a copy or doesn't belong to this user")
            );

        Assert.All(currentBorrowedCopylBooksByTheSameUser,
            bo => Assert.True(bo.IsReturned == false && bo.IsCopy == true && bo.UserId == userId));

        Assert.Equal(expectedBorrowOrders(currentBorrowedOriginalBooksByTheSameUser, currentBorrowedCopylBooksByTheSameUser), library.BorrowedBooks.Count);
    }

    [Fact]
    public void BorrowBook_BorrowOriginalBook_Should_Throw_WhenTheSameUserTriesToBorrowMoreThanTwoBooks()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs.ElementAt(0)),
                CreateBaseBook(bookISBNs.ElementAt(1)),
                CreateBaseBook(bookISBNs.ElementAt(2)),
                CreateBaseBook(bookISBNs.ElementAt(3))
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);
        library.BorrowBook(bookISBNs.ElementAt(1), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Action<Entities.Library> BorrowOriginal
            = (Entities.Library lib) => lib.BorrowBook(bookISBNs.ElementAt(3), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Func<Entities.Library, List<BorrowOrder>> UserHasTwoOriginalBooksUnreturned =
            (Entities.Library lib) => lib.BorrowedBooks.Where(bo => bo.UserId == userId && bo.IsCopy == false && bo.IsReturned == false).ToList();

        var unreturnedBooks = UserHasTwoOriginalBooksUnreturned(library);

        // Act & Assert        
        Assert.Throws<InvalidLibraryStateException>(() => BorrowOriginal(library));
        Assert.Equal(2, unreturnedBooks.Count());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(1)]
    public void BorrowBook_Should_BorrowCopyBook_WhenTheSameUserHaveNotHaveBorrowedTheSameCopyOfABookMoreThanOnce(int currentAmountBorrowed)
    {
        // Arrange
        int expectedCount = currentAmountBorrowed + 1;
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        Action<Entities.Library, string> BorrowCopy =
            (Entities.Library lib, string bookISBN) =>
            lib.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowCopyOrder);

        Queue<Action> borrowQueue = new Queue<Action>(4);

        for (int i = 0; i < currentAmountBorrowed; i++)
        {
            if (i == 0) borrowQueue.Enqueue(
                () => BorrowCopy(library, bookISBNs[0])
                );

            if (i == 1) borrowQueue.Enqueue(
                () => BorrowCopy(library, bookISBNs[1])
                );

            if (i == 2) borrowQueue.Enqueue(
                () => BorrowCopy(library, bookISBNs[2])
                );

        }

        foreach (var borrowAction in borrowQueue)
        {
            borrowAction();
        }

        // Act
        library.BorrowBook(bookISBNs.ElementAt(3), userId, BorrowOrderPetition.BorrowCopyOrder);

        var currentBorrowedBooksByTheSameUser =
            library.BorrowedBooks.Where(bo =>
            bo.IsReturned == false &&
            bo.IsCopy == true &&
            bo.UserId == userId);

        // Assert
        Assert.Equal(expectedCount, currentBorrowedBooksByTheSameUser.Count());
        Assert.All(currentBorrowedBooksByTheSameUser,
            bo => Assert.True(bo.IsReturned == false && bo.IsCopy == true && bo.UserId == userId,
            "BorrowOrder is either returned, an original or doesn't belong to this user")
            );
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    public void BorrowBook_Should_BorrowCopyBook_WhenTheSameUserHaveNotHaveBorrowedMoreThanTwoBooks_AndHasSeveralDifferentCopiesUnreturned
        (int currentOriginalBorrowed, int currentCopiesBorrowed)
    {
        // Arrange
        int expectedCount = currentCopiesBorrowed + 1;
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        Action<Entities.Library, string, BorrowOrderPetition> BorrowBook =
            (Entities.Library lib, string bookISBN, BorrowOrderPetition petition) =>
            lib.BorrowBook(bookISBN, userId, petition);

        Queue<Action> borrowQueue = new Queue<Action>(7);
        for (int i = 0; i < currentOriginalBorrowed; i++)
        {
            if (i == 0) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[0], BorrowOrderPetition.BorrowOriginalOrder)
                );

            if (i == 1) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[1], BorrowOrderPetition.BorrowOriginalOrder)
                );

            if (i == 2) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[2], BorrowOrderPetition.BorrowOriginalOrder)
                );

        }

        for (int i = 0; i < currentCopiesBorrowed; i++)
        {
            if (i == 0) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[0], BorrowOrderPetition.BorrowCopyOrder)
                );

            if (i == 1) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[1], BorrowOrderPetition.BorrowCopyOrder)
                );

            if (i == 2) borrowQueue.Enqueue(
                () => BorrowBook(library, bookISBNs[2], BorrowOrderPetition.BorrowCopyOrder)
                );

        }

        foreach (var borrowAction in borrowQueue)
        {
            borrowAction();
        }

        // Act
        library.BorrowBook(bookISBNs.ElementAt(3), userId, BorrowOrderPetition.BorrowCopyOrder);

        var currentBorrowedBooksByTheSameUser =
            library.BorrowedBooks.Where(bo =>
            bo.IsReturned == false &&
            bo.IsCopy == true &&
            bo.UserId == userId);

        // Assert
        Assert.Equal(expectedCount, currentBorrowedBooksByTheSameUser.Count());
        Assert.All(currentBorrowedBooksByTheSameUser,
            bo => Assert.True(bo.IsReturned == false && bo.IsCopy == true && bo.UserId == userId,
            "BorrowOrder is either returned, an original or doesn't belong to this user")
            );
    }

    [Fact]
    public void BorrowBook_BorrowCopyBook_Should_Throw_WhenTheSameUserTriesToBorrowTheSameBookMoreThanOnce_WhenIsUnreturned()
    {
        // Arrange 
        int originalAmount = 50;
        int copiesAmount = 20;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-2-8340-6328-4";

        BorrowOrder borrowOrder = BorrowOrder.Create(userId, bookISBN);

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBN, original: originalAmount, copies: copiesAmount),
                CreateBaseBook()
            }
        };

        library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowCopyOrder);

        Action<Entities.Library> BorrowSameBookAgain =
            (Entities.Library library) => library.BorrowBook(bookISBN, userId, BorrowOrderPetition.BorrowCopyOrder);

        // Act & Assert
        Assert.Throws<InvalidLibraryStateException>(() => BorrowSameBookAgain(library));
    }

    [Fact]
    public void ReturnBook_Should_ReturnTheSameOriginalBookBorrowed_WhenIsReturned()
    {
        // Arramge
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Func<Entities.Library, Guid, List<BorrowOrder>> DoesThisUserHaveABookByThisIdUnreturned
            = (Entities.Library lib, Guid userId) => lib.BorrowedBooks
            .Where(
                bo => bo.UserId == userId &&
                bo.BookISBN == bookISBNs.ElementAt(0) &&
                bo.IsReturned == false
                )
            .ToList();

        // Act
        library.ReturnBook(bookISBNs, userId);

        // Assert
        Assert.All(DoesThisUserHaveABookByThisIdUnreturned(library, userId),
            bo => Assert.True(bo.UserId != userId));
    }

    [Fact]
    public void ReturnBook_Should_ReturnTheSameCopyBookBorrowed_WhenIsReturned()
    {
        // Arramge
        Guid userId = Guid.NewGuid();
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowCopyOrder);

        Func<Entities.Library, Guid, List<BorrowOrder>> DoesThisUserHaveABookByThisIdUnreturned
            = (Entities.Library lib, Guid userId) => lib.BorrowedBooks
            .Where(
                bo => bo.UserId == userId &&
                bo.BookISBN == bookISBNs.ElementAt(0) &&
                bo.IsReturned == false
                )
            .ToList();

        // Act
        library.ReturnBook(bookISBNs, userId);

        // Assert
        Assert.All(DoesThisUserHaveABookByThisIdUnreturned(library, userId),
            bo => Assert.True(bo.UserId != userId));
    }

    [Theory]
    [InlineData(15)]
    [InlineData(25)]
    [InlineData(5000)]
    [InlineData(1)]
    public void ReturnBook_Should_OnlyIncreaseOriginalStockAmount_AfterOriginalIsReturned(int stockAmount)
    {
        // Arrange
        Book book;
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0], original: stockAmount
                ),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Func<Entities.Library, Guid, List<BorrowOrder>> DoesThisUserHaveABookByThisIdUnreturned
            = (Entities.Library lib, Guid userId) => lib.BorrowedBooks
            .Where(
                bo => bo.UserId == userId &&
                bo.BookISBN == bookISBNs.ElementAt(0) &&
                bo.IsReturned == false
                )
            .ToList();

        // Act
        library.ReturnBook(new List<string>() { "978-2-8340-6328-4" }, userId);
        book = library.Books.Where(bo => bo.ISBN == bookISBNs.ElementAt(0)).First();

        // Assert
        Assert.Equal(stockAmount, book.Stock.OriginalAmount);
        Assert.All(DoesThisUserHaveABookByThisIdUnreturned(library, userId),
            bo => Assert.True(bo.UserId != userId));
    }

    [Theory]
    [InlineData(15)]
    [InlineData(25)]
    [InlineData(5000)]
    [InlineData(1)]
    public void ReturnBook_Should_OnlyIncreaseCopiesStockAmount_AfterCopyIsReturned(int stockAmount)
    {
        // Arrange
        Book book;
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0], copies: stockAmount
                ),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowCopyOrder);

        Func<Entities.Library, Guid, List<BorrowOrder>> DoesThisUserHaveABookByThisIdUnreturned
            = (Entities.Library lib, Guid userId) => lib.BorrowedBooks
            .Where(
                bo => bo.UserId == userId &&
                bo.BookISBN == bookISBNs.ElementAt(0) &&
                bo.IsReturned == false
                )
            .ToList();

        // Act
        library.ReturnBook(new List<string>() { "978-2-8340-6328-4" }, userId);
        book = library.Books.Where(bo => bo.ISBN == bookISBNs.ElementAt(0)).First();

        // Assert
        Assert.Equal(stockAmount, book.Stock.CopiesAmount);
        Assert.All(DoesThisUserHaveABookByThisIdUnreturned(library, userId),
            bo => Assert.True(bo.UserId != userId));
    }

    [Fact]
    public void ReturnBook_Should_Throw_WhenTheUserHasNoCurrentUnreturnedBooks()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        Action<Entities.Library> ReturnBook
            = (Entities.Library lib) => lib.ReturnBook(
                new List<string>() { bookISBNs.ElementAt(0) }, userId);

        // Act & Assert
        Assert.Throws<InvalidLibraryOperationException>(() => ReturnBook(library));
    }

    [Fact]
    public void ReturnBook_Should_Throw_WhenISBNsInputAreNullOrEmpty()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Action<Entities.Library> ReturnBook
            = (Entities.Library lib) => lib.ReturnBook(
                new List<string>(3) { "", null, "" }, userId);

        // Act & Assert
        Assert.Throws<InvalidInputException>(() => ReturnBook(library));
    }

    [Fact]
    public void ReturnBook_Should_Throw_WhenUserInputIsNullOrEmpty()
    {
        // Arrange
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), Guid.NewGuid(), BorrowOrderPetition.BorrowOriginalOrder);

        Action<Entities.Library> ReturnBook
            = (Entities.Library lib) => lib.ReturnBook(
                new List<string>(3) { bookISBNs.ElementAt(0) }, Guid.Empty);

        // Act & Assert
        Assert.Throws<InvalidInputException>(() => ReturnBook(library));
    }

    [Fact]
    public void ReturnBook_Should_Throw_WhenInputISBNDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Action<Entities.Library> ReturnBook
            = (Entities.Library lib) => lib.ReturnBook(
                new List<string>(3) { "123-4-5432-4217-4" }, userId);

        // Act & Assert
        Assert.Throws<InvalidLibraryStateException>(() => ReturnBook(library));
    }

    [Fact]
    public void ReturnBook_Should_Throw_WhenNoUnreturnedBooksExistByGivenISBN()
    {
        // Arrange
        Guid userId = Guid.NewGuid();

        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var library = new Entities.Library()
        {
            Books = {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
            }
        };

        library.BorrowBook(bookISBNs.ElementAt(0), userId, BorrowOrderPetition.BorrowOriginalOrder);

        Action<Entities.Library> ReturnBook
            = (Entities.Library lib) => lib.ReturnBook(
                new List<string>(3) { bookISBNs.ElementAt(3) }, userId);

        // Act & Assert
        Assert.Throws<InvalidLibraryStateException>(() => ReturnBook(library));
        Assert.Contains(library.Books, b => b.ISBN == bookISBNs.ElementAt(3));
    }

    [Fact]
    public void CreateFromBooks_Should_InitializeLibraryFromBookCollection()
    {
        // Arrange
        Domain.Entities.Library library;
        List<string> bookISBNs = new List<string>(4)
        {
            "978-2-8340-6328-4",
            "978-8-9334-4497-9",
            "978-1-9334-4497-9",
            "978-4-0183-6798-0"
        };

        var books = new HashSet<Book>()
        {
                CreateBaseBook(bookISBNs[0]),
                CreateBaseBook(bookISBNs[1]),
                CreateBaseBook(bookISBNs[2]),
                CreateBaseBook(bookISBNs[3])
        };

        // Act
        library = Domain.Entities.Library.CreateFromBooks(books);

        // Assert
        Assert.NotNull(library);
        Assert.All(library.Books, 
            b => Assert.False(string.IsNullOrWhiteSpace(b.ISBN)));
        Assert.Empty(library.BorrowedBooks);
    }

    [Fact]
    public void CreateFromBooks_Should_Throw_WhenBookCollectionIsNull()
    {
        // Arrange
        Domain.Entities.Library library = null;

        var books = new HashSet<Book>()
        {
                null,
                null,
                null,
                null
        };

        Action<Domain.Entities.Library, HashSet<Book>> CreateLibraryFromBooks = 
            (lib, books) => lib = Domain.Entities.Library.CreateFromBooks(books);

        // Act & Assert
        Assert.Throws<AggregateException>(() => CreateLibraryFromBooks(library, books));
        Assert.Null(library);
    }

    [Fact]
    public void CreateFromBooks_Should_Throw_WhenBookCollectionIsEmpty()
    {
        // Arrange
        Domain.Entities.Library library = null;

        var books = new HashSet<Book>()
        {
  
        };

        Action<Domain.Entities.Library, HashSet<Book>> CreateLibraryFromBooks =
            (lib, books) => lib = Domain.Entities.Library.CreateFromBooks(books);

        // Act & Assert
        Assert.Throws<InvalidLibraryOperationException>(() => CreateLibraryFromBooks(library, books));
        Assert.Null(library);
    }


    private static Book CreateBaseBook(string isbn = "978-1-0217-2611-7", int original = 15, int copies = 30)
    {

        string author = "Gaspar Noe";
        string title = "Black Swan";
        DateTime yearPubllished = DateTime.Now.AddYears(Random.Shared.Next(-60, -20));
        BookStock stock = BookStock.Create(original, copies);

        return Book.Create(isbn, author, title, yearPubllished, stock);
    }

    private static Book CreateBaseBook(
        DateTime yearPubllished,
        string isbn = "978-1-0217-2611-7",
        string author = "Gaspar Noe",
        string title = "Black Swan",
        int original = 15,
        int copies = 30)
    {

        BookStock stock = BookStock.Create(original, copies);


        return Book.Create(isbn, author, title, yearPubllished, stock);

    }
}

