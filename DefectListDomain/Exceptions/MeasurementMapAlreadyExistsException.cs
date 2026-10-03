using System;

namespace DefectListDomain.Exceptions
{
    public class MeasurementMapAlreadyExistsException : Exception
    {
        public MeasurementMapAlreadyExistsException(string message) : base(message) { }
    }
}