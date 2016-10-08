using System;

namespace Jarvis
{
    public sealed class IntentExecutorAttribute : Attribute
    {
        public string Name { get; set; }

        public IntentExecutorAttribute(string name)
        {
            Name = name;
        }
    }
}