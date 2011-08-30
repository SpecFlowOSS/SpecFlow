using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    [Serializable]
    public class FileGeneratorException : Exception
    {
        public FileGeneratorException(string message)
            : base(message)
        {
        }

        public FileGeneratorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FileGeneratorException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
