using System;

namespace Common.Infrastructure.Exceptions
{
    public class ApplicationCustomException : ApplicationException
    {
        public int InternalCode { get; set; }

        public ApplicationCustomException(string message) : base(message)
        {
        }

        public ApplicationCustomException(string message, int internalCode) : base(message)
        {
            InternalCode = internalCode;
        }
    }
}
