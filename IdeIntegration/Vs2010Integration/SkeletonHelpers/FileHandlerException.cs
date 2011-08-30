using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    [Serializable]
    public class FileHandlerException : Exception
    {
        public FileHandlerException(string message)
            : base(message)
        {
        }

        public FileHandlerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FileHandlerException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
