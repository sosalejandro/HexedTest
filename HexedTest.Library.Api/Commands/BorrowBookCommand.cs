namespace HexedTest.Library.Api.Commands;

public record BorrowBookCommand(string ISBN, Guid UserId, BorrowOrderPetition Petition);

