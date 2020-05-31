namespace Cassandra.Fluent.Migrator.Utils.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Cassandra.Fluent.Migrator.Utils.Extensions;

    /// <summary>
    /// Exceptions that's thrown when an object was not found.
    /// </summary>
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException()
        {
        }

        public ObjectNotFoundException(string message)
            : base(message.NormalizeString())
        {
        }

        public ObjectNotFoundException(string message, Exception innerException)
            : base(message.NormalizeString(), innerException)
        {
        }

        protected ObjectNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
