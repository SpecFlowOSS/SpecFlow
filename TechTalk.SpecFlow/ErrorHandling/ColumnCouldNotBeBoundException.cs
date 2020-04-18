using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


// the exceptions are part of the public API, keep them in TechTalk.SpecFlow namespace
namespace TechTalk.SpecFlow
{
    [Serializable]
    public class ColumnCouldNotBeBoundException : Exception
    {
        public IList<string> Columns { get; }

        /// <inheritdoc />
        public ColumnCouldNotBeBoundException()
        {
        }

        /// <inheritdoc />
        public ColumnCouldNotBeBoundException(IList<string> columns) : base(CreateMessage(columns))
        {
            Columns = columns;
        }

        /// <inheritdoc />
        public ColumnCouldNotBeBoundException(string message, IList<string> columns) : base(message)
        {
            Columns = columns;
        }

        /// <inheritdoc />
        public ColumnCouldNotBeBoundException(string message, Exception innerException, IList<string> columns) : base(message, innerException)
        {
            Columns = columns;
        }

        /// <inheritdoc />
        protected ColumnCouldNotBeBoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string CreateMessage(IList<string> columns)
        {
            if (columns.Count == 1)
            {
                return $"Member or field {string.Join(",", columns)} not found";
            }
            return $"Members or fields {string.Join(",", columns)} not found";

        }
    }
}
