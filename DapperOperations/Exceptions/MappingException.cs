using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DapperOperations.Exceptions
{
    /// <summary>
    /// Represents an error for some mapping problem
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MappingException : Exception
    {
        /// <summary>
        /// Initialize a new instance of <see cref="MappingException"/>
        /// </summary>
        public MappingException()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MappingException"/> with an error message
        /// </summary>
        /// <param name="message">Error message</param>
        public MappingException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MappingException"/> with an error message and an inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public MappingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MappingException"/> with a serialization info and a context
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Context</param>
        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
