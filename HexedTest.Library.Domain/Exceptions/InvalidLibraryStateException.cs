using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Exceptions;

 public class InvalidLibraryStateException : Exception
{
    public InvalidLibraryStateException(string message) : base(message)
    {

    }
}
