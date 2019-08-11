using System;

namespace Vim.G3D
{
    public class Attribute : INamedBuffer
    {
        public Attribute(AttributeDescriptor desc, IBuffer data)
        {
            Descriptor = desc;
            Data = data;
        }

        public string Name => Descriptor.ToString();
        public Span<byte> Bytes => Data.Bytes;

        public AttributeDescriptor Descriptor { get; }
        public IBuffer Data { get; }
    }

    public static class AttributeExtensions
    {
        public static Attribute ToAttribute(this INamedBuffer buffer)
            => new Attribute(AttributeDescriptor.Parse(buffer.Name), buffer);
    }
}
