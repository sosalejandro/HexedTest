# HexedTest Library

Architectural Design: Event Sourcing - CQRS 
Language: C#
Tech-Stack: SQL Server, CosmosDB, .NET 6, Azure Functions.
Implementation: Tactical Design

## Installation

For usage of the application it is needed the .NET runtime, a Microsoft SQL Server instance and internet connection.

## Usage

within the Api projects and the Projector they can be run with the command
```sh
dotnet run
```
Beware of the keys to the Azure CosmosDB, the service will be running for the next week from now and after the next friday (March 18th) they will be deleted.

## Thoughts

- Language: When approaching to develop the application Go and JavaScript came into to mind in the very early stages (prior to reading the specifications). Since the specifications were for an app with clean code and TDD then only Go and C# came into my mind. Leaving me with C# in the very end because of both are pretty similar in performance but C# has incredible goodies I can get with Visual Studio while developing.

- Design: When choosing the design a simply because of the nature of the system (borrowing/returns) I'd like to keep a tracking of the system for any reason so an event sourcing system came into mind. Now, the domain itself did evolve during the early stages before commiting the initial library object due to I knew I was designing the object and until I had a clear "final" version, an object to which I will stick to develop, a business that I found that could fit to match all criterias. Early developing I was thinking of 2 methods for borrowing then I went with a factory approach, delegating how to handle it to the request and the object than to a concrete implementation left for public usage. This being later used in the controller to create a single endpoint which handles 2 different use cases.  

Early stages of the progam had notes like: 
```sh
1. GET - Library/Books
Return Books[] = [Book, Book, Book]
Return Books[] = []

2. POST - Library/Withdraw?book={bookId}&user={userId}
Return - No Content success
Return - Bad Request

3. POST - Library/Withdraw?bookCopy={bookId}&user={userId}
Return - No Content success
Return - Bad Request

4. UPDATE - Library/Deposit?book={bookId}&user={userId}
			Library/Deposit?bookCopy={bookId}&user={userId}
```

Which would be something close to the final result through the usage of an ApiGateway, but the application now doesn't use query params but a body object within the request for handling the endpoints. 

Other notes such as the specs thoughts:
```sh
A library has books

1. An user can see the books at the library:
If books.count > 0, return books
if books.count = 0, return empty library

2. An user can borrow books
If books.count > 0, user can borrow book, then books is added to borrowed list, and the book is removed from the library.
If books.count == 0, user can't borrow books
### An user can only borrow 2 books at a time.

3. User can borrow a copy of book
A user can borrow copies of books, it is added to the borrowed list, the library still displays books if there is any to show.
A user borrows the last copy of a book in the library, it is added to the borrowed list, then the book is removed from the library.
### An user can only borrow 1 copy of a book at any time.

4. User return books
2 books in borrowed list, return 1, list displays 1 book left, the library updates with 1 book returned.
2 books in borrowed list, return 2, list is emptied, library updates with 2 books returned.
```

Particularly the implementation of the return method suffered a drastic change. 
From handling a single ISBN at a time, to handle an abstract return of just looking up the user by the amount of books they want to return and the amount of books the user has unreturned, to just typing a list of strings which should match ISBNs within the library and that the user has borrowed. So only by using the ISBN the book will be returned, the program will handle whether it is a copy or an original as long it is unreturned by this user. But on the other hand copies and books do not collision with themselves so this can lead to unexpected results when this case is true, but this given case is something I did't leave open for interpreation so I didn't create any validation for this case for now, it isn't specified to cover this case so I think it wasn't foreseen or it is intended.  

## Troubleshoot

If the projector ever gives a error for worker.config.json the file it is missing must look similar to this. And it should go within the bin/debug/net6.0 folder 

worker.config.json
```json
{
    "description": {
      "language": "dotnet-isolated",
      "extensions": [ ".dll" ],
      "defaultExecutablePath": "dotnet",
      "defaultWorkerPath": "HexedTest.Library.Projector.dll"
    }
  }
  
```

## License
[MIT](https://choosealicense.com/licenses/mit/)