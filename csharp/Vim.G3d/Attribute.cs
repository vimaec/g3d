using System;
using System.Runtime.InteropServices;

namespace Vim.G3d
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

    public class Attribute<T> : INamedBuffer where T: struct
    {
        public Attribute _Attribute { get; }
        public Span<T> Data => _Attribute.CastData<T>();
        public string Name => _Attribute.Name;
        public Span<byte> Bytes => _Attribute.Bytes;
    }    

    public static class AttributeExtensions
    {
        public static Attribute ToAttribute(this INamedBuffer buffer)
            => new Attribute(AttributeDescriptor.Parse(buffer.Name), buffer);

        public static Span<T> CastData<T>(this Attribute attr) where T : struct
            => MemoryMarshal.Cast<byte, T>(attr.Bytes);

        public static T[] ToArray<T>(this Attribute<T> attr) where T : struct
            => attr == null ? null : attr.Data.ToArray();

        public IBuffer ToBuffer<T>(this T[] data) where T : struct
            => data.AsMemory();

        public Attribute<T> ToAttribute<T>(this T[] data, AssociationEnum assoc, SemanticEnum sem, DataTypeEnum dt, int arity) where T: struct
        {
        }
    }
}
