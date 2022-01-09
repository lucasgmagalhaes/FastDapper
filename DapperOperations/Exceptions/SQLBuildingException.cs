using System.Runtime.Serialization;

namespace DapperOperations.Exceptions
{
    /// <summary>
    /// Represents an error for any build process problem that may occour in sql building
    /// </summary>
    [Serializable]
    public class SqlBuildingException : Exception
    {
        /// <summary>
        /// Initialize a new instance of <see cref="SqlBuildingException"/>
        /// </summary>
        public SqlBuildingException()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SqlBuildingException"/> with a message
        /// </summary>
        /// <param name="message"></param>
        public SqlBuildingException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SqlBuildingException"/> with a message and an inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public SqlBuildingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="SqlBuildingException"/> with a serialization info
        /// and a context
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Context</param>
        protected SqlBuildingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
