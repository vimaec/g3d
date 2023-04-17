using System;
using System.Linq.Expressions;

namespace Vim.G3d
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AttributeDescriptorAttribute : Attribute
    {
        public string Name { get; set; }
        public Type ArrayType { get; set; } = null;
        public AttributeType AttributeType { get; set; } = AttributeType.Data;
        public Type IndexInto { get; set; } = null;

        public AttributeDescriptorAttribute(string name, AttributeType attributeType)
        {
            Name = name;
            AttributeType = attributeType;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AttributeCollectionAttribute : Attribute
    {
        public Type[] AttributeClasses { get; }

        public AttributeCollectionAttribute(params Type[] attributeClasses)
            => AttributeClasses = attributeClasses;
    }
}