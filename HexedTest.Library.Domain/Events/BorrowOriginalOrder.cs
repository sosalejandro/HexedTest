using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Events;

public record BorrowOriginalOrder(string ISBN, Guid UserId) : IDomainEvent { }