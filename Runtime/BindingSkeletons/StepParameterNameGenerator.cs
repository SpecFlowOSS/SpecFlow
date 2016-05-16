using System.Collections.Generic;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public class StepParameterNameGenerator
    {
        public static  string GenerateParameterName(string value,int paramIndex, List<string> usedParameterNames)
        {
            if (value.IsSingleWordWithNoNumbers())
                return value.WithLowerCaseFirstLetter().UniquelyIdentified(usedParameterNames,paramIndex);
            if (value.IsSingleWordSurroundedByAngleBrackets())
                return value.WithoutSurroundingAngleBrackets().WithLowerCaseFirstLetter().UniquelyIdentified(usedParameterNames, paramIndex);
            return "p" + paramIndex;
        }
    }
}