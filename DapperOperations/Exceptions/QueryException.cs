using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DapperOperations.Exceptions
{
    /// <summary>
    /// Represents an error that occours when executing a query
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class QueryException : Exception
    {
        /// <summary>
        /// Initializes the exception without message or inner exception
        /// </summary>
        public QueryException()
        {
        }

        /// <summary>
        /// Initializes the exception with a message
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public QueryException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initializes the expcetion with an message and an inner exception
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Inner exception of the exception</param>
        public QueryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes the exception with an info for it's serialization and a context
        /// </summary>
        /// <param name="info">Info of the exception</param>
        /// <param name="context">Context of the exception</param>
        protected QueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
