
namespace System.ComponentModel
{
    using System;

    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }
    }
}
