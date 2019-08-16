using System;
using System.Runtime.InteropServices;

namespace Vim.G3D
{
    public class Attribute : INamedBuffer
    {
        public Attribute(AttributeDescriptor desc, IBuffer data)
        {
            Descriptor = desc;
            Data = data;
            if (data.Bytes.Length % desc.DataElementSize != 0)
                throw new Exception("Number of bytes in buffer is not divided evenly by the size of elements");
            Count = data.Bytes.Length / desc.DataElementSize;
        }

        public string Name => Descriptor.ToString();
        public Span<byte> Bytes => Data.Bytes;
        public int Count { get; }

        public AttributeDescriptor Descriptor { get; }
        public IBuffer Data { get; }
    }

    public static class AttributeExtensions
    {
        public static Attribute ToAttribute(this INamedBuffer buffer)
            => new Attribute(AttributeDescriptor.Parse(buffer.Name), buffer);

        public static Span<T> CastData<T>(this Attribute attr) where T : struct
            => MemoryMarshal.Cast<byte, T>(attr.Bytes);
    }
}
