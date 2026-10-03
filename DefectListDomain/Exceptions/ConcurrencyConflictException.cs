using System;

namespace DefectListDomain.Exceptions
{
    public sealed class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message) : base(message) { }
    }
}