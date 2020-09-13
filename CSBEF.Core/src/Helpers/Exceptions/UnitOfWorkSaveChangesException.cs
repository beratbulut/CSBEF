using System;

namespace CSBEF.Core.Helpers.Exceptions
{
    [Serializable]
    public class UnitOfWorkSaveChangesException : Exception
    {
        public UnitOfWorkSaveChangesException(string message) : base(message)
        {
        }

        public UnitOfWorkSaveChangesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnitOfWorkSaveChangesException()
        {
        }

        protected UnitOfWorkSaveChangesException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }
}