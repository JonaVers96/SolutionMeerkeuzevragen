using System;
namespace MeerkeuzevragenBL.Exceptions
{
    public class DomeinException : Exception
    {
        public DomeinException(string message) : base(message) { }
        public DomeinException(string message, Exception innerException) : base(message, innerException) { }

    }
}
