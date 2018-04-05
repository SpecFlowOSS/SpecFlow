using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IWriter
    {
        string WriteIfNotEqual(string content);
    }
}
