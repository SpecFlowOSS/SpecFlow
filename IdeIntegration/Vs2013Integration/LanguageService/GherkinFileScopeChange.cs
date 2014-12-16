using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class GherkinFileScopeChange : EventArgs
    {
        public IGherkinFileScope GherkinFileScope { get; private set; }

        public bool LanguageChanged { get; private set; }
        public bool EntireScopeChanged { get; private set; }

        public IEnumerable<IGherkinFileBlock> ChangedBlocks { get; private set; }
        public IEnumerable<IGherkinFileBlock> ShiftedBlocks { get; private set; }

        public GherkinFileScopeChange(IGherkinFileScope gherkinFileScope, bool languageChanged, bool entireScopeChanged, IEnumerable<IGherkinFileBlock> changedBlocks, IEnumerable<IGherkinFileBlock> shiftedBlocks)
        {
            GherkinFileScope = gherkinFileScope;
            LanguageChanged = languageChanged;
            EntireScopeChanged = entireScopeChanged;
            ChangedBlocks = changedBlocks;
            ShiftedBlocks = shiftedBlocks;
        }

        internal static GherkinFileScopeChange CreateEntireScopeChange(IGherkinFileScope fileScope)
        {
            return new GherkinFileScopeChange(fileScope, true, true, fileScope.GetAllBlocks(), Enumerable.Empty<IGherkinFileBlock>());
        }
    }
}