using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Tag
    {
        public string Name { get; set; }

        public Tag()
        {
        }

        public Tag(Word name)
        {
            Name = name.Value;
        }
    }

    public class Tags : List<Tag>
    {
        public Tags()
        {
        }

        public Tags(params Tag[] tags)
        {
            AddRange(tags);
        }
    }
}