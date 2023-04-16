using System;
using System.Runtime.Serialization;

namespace BlazamSetup.Services
{
    [Serializable]
    internal class ApplicationUpdateException : Exception
    {
        public ApplicationUpdateException()
        {
        }

        public ApplicationUpdateException(string message) : base(message)
        {
        }

        public ApplicationUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}