using System;

namespace Experimentation.Persistence.Attributes
{
    /// <summary>
    /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
    /// is not specified, the classname will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionNameAttribute : Attribute
    {
        public ConnectionNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty connection name is not allowed", nameof(value));

            Name = value;
        }

        public virtual string Name { get; private set; }
    }
}