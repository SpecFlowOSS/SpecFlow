using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace IdeIntegrationTests
{
    class GoToDefinitionHelper
    {
        private IEnumerable<System.Reflection.MethodInfo> _bindings;

        public GoToDefinitionHelper(IEnumerable<System.Reflection.MethodInfo> bindings)
        {
            _bindings = bindings;
        }

        internal IEnumerable<MethodInfo> GetMethodsMatchingTextAtCaret(string input, EditorContext editorContext)
        {
            return _bindings;
        }
    }
}
