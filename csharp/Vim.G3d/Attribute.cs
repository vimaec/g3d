using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Vim.G3d
{
    public class BinaryAttribute : INamedBuffer
    {
        public BinaryAttribute(AttributeDescriptor desc, IBuffer data)
        {
            Descriptor = desc;
            Data = data;
            if (data.Bytes.Length % desc.DataElementSize != 0)
                throw new Exception("Number of bytes in buffer is not divided evenly by the size of elements");
            ElementCount = data.Bytes.Length / desc.DataElementSize;
            PrimitiveCount = ElementCount / desc.DataArity;
        }

        public string Name => Descriptor.ToString();
        public Span<byte> Bytes => Data.Bytes;
        public int ElementCount { get; }
        public int PrimitiveCount { get; }

        public AttributeDescriptor Descriptor { get; }
        public IBuffer Data { get; }
    }

    public class Attribute<T> : INamedBuffer where T: struct
    {
        public static void TypeCheck(params Type[] types)
        {
            if (types.All(t => typeof(T) != t))
                throw new Exception($"Incompatible type {typeof(T)} expected {string.Join<Type>(",", types)}");
        }

        public Attribute(BinaryAttribute attr)
        {
            switch (attr.Descriptor.DataType)
            {
                case DataType.dt_float32:
                    TypeCheck(typeof(float));
                    break;
                case DataType.dt_float64:
                    TypeCheck(typeof(double));
                    break;
                case DataType.dt_int8:
                case DataType.dt_uint8:
                    TypeCheck(typeof(byte), typeof(sbyte));
                    break;
                case DataType.dt_int16:
                case DataType.dt_uint16:
                    TypeCheck(typeof(short), typeof(ushort));
                    break;
                case DataType.dt_int32:
                case DataType.dt_uint32:
                    TypeCheck(typeof(int), typeof(uint));
                    break;
                case DataType.dt_int64:
                case DataType.dt_uint64:
                    TypeCheck(typeof(long), typeof(ulong));
                    break;
                default:
                    throw new Exception("Unrecognized data type");
            }
            _Attribute = attr;
        }

        public BinaryAttribute _Attribute
        { get; }

        public Span<T> Data 
            => _Attribute.CastData<T>();

        public string Name 
            => _Attribute.Name;

        public Span<byte> Bytes 
            => _Attribute.Bytes;

        public AttributeDescriptor Descriptor 
            => _Attribute.Descriptor;

        public int ElementCount 
            => _Attribute.ElementCount;

        public int PrimitiveCount 
            => _Attribute.PrimitiveCount;

        public Span<U> CastData<U>() where U: struct
            => _Attribute.CastData<U>();
    }        

    public static class AttributeExtensions
    {
        public static BinaryAttribute ToAttribute(this INamedBuffer buffer)
            => new BinaryAttribute(AttributeDescriptor.Parse(buffer.Name), buffer);

        public static Span<T> CastData<T>(this BinaryAttribute attr) where T : struct
            => MemoryMarshal.Cast<byte, T>(attr.Bytes);

        public static T[] ToArray<T>(this Attribute<T> attr) where T : struct
            => attr?.Data.ToArray();

        public static IBuffer ToBuffer<T>(this T[] data) where T : struct
            => data.AsMemory().ToBuffer();

        public static Attribute<T> ToAttribute<T>(this T[] data, string name) where T : struct
            => data.ToBinaryAttribute(name).AsType<T>();

        public static BinaryAttribute ToBinaryAttribute<T>(this T[] data, string name) where T : struct
            => data.ToNamedBuffer(name).ToAttribute();

        public static Attribute<T> AsType<T>(this BinaryAttribute attr) where T : struct
            => new Attribute<T>(attr);

        public static Attribute<T> CheckArity<T>(this Attribute<T> self, int arity) where T : struct
            => self?.Descriptor?.DataArity == arity ? self : null;

        public static Attribute<T> CheckAssociation<T>(this Attribute<T> self, Association assoc) where T : struct
            => self?.Descriptor?.Association == assoc ? self : null;

        public static Attribute<T> CheckArityAndAssociation<T>(this Attribute<T> self, int arity, Association assoc) where T : struct
            => self?.CheckArity(arity)?.CheckAssociation(assoc);
    }
}
