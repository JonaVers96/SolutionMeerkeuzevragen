using System;

namespace MeerkeuzevragenBL.Exceptions
{
    public class ManagerException : Exception
    {
        public ManagerException(string message) : base(message) { }
        public ManagerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
