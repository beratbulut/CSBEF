using System;

namespace CSBEF.src.Helpers.Exceptions
{
    [Serializable]
    public class DynamicServiceActionException : Exception
    {
        public DynamicServiceActionException(string message) : base(message)
        {

        }

        public DynamicServiceActionException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public DynamicServiceActionException()
        {

        }

        protected DynamicServiceActionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }
}