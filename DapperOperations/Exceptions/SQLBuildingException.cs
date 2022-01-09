using System.Runtime.Serialization;

namespace DapperOperations.Exceptions
{
    [Serializable]
    public class SQLBuildingException : Exception
    {
        public SQLBuildingException()
        {
        }

        public SQLBuildingException(string? message) : base(message)
        {
        }

        public SQLBuildingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SQLBuildingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
