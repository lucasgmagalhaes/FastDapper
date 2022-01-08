using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace DapperOperations.Exceptions
{
    [Serializable]
    public class MappingException : Exception
    {
        public MappingException()
        {
        }

        public MappingException(string? message) : base(message)
        {
        }

        public MappingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool Equals(object? obj)
        {
            return obj is MappingException exception &&
                   EqualityComparer<IDictionary>.Default.Equals(Data, exception.Data) &&
                   HelpLink == exception.HelpLink &&
                   HResult == exception.HResult &&
                   EqualityComparer<Exception?>.Default.Equals(InnerException, exception.InnerException) &&
                   Message == exception.Message &&
                   Source == exception.Source &&
                   StackTrace == exception.StackTrace &&
                   EqualityComparer<MethodBase?>.Default.Equals(TargetSite, exception.TargetSite);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, HelpLink, HResult, InnerException, Message, Source, StackTrace, TargetSite);
        }
    }
}
