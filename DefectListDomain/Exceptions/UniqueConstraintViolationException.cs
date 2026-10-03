using System;

namespace DefectListDomain.Exceptions
{
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(string message) : base(message) { }
    }
}