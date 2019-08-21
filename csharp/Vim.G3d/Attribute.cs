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
        public static void TypeCheck(params Type[] types)
        {
            if (!types.Any(t => typeof(T) == t))
                throw new Exception($"Incompatible type {typeof(T)} expected {string.Join<Type>(",", types)}");
        }

        public Attribute(BinaryAttribute attr)
        {
            switch (attr.Descriptor.DataType)
            {
                case DataTypeEnum.dt_float32:
                    TypeCheck(typeof(float));
                    break;
                case DataTypeEnum.dt_float64:
                    TypeCheck(typeof(double));
                    break;
                case DataTypeEnum.dt_int8:
                case DataTypeEnum.dt_uint8:
                    TypeCheck(typeof(byte), typeof(sbyte));
                    break;
                case DataTypeEnum.dt_int16:
                case DataTypeEnum.dt_uint16:
                    TypeCheck(typeof(short), typeof(ushort));
                    break;
                case DataTypeEnum.dt_int32:
                case DataTypeEnum.dt_uint32:
                    TypeCheck(typeof(int), typeof(uint));
                    break;
                case DataTypeEnum.dt_int64:
                case DataTypeEnum.dt_uint64:
                    TypeCheck(typeof(long), typeof(ulong));
                    break;
                default:
                    throw new Exception("Unrecognized data type");
            }
            _Attribute = attr;
        }

        public BinaryAttribute _Attribute { get; }
        public Span<T> Data => _Attribute.CastData<T>();
        public string Name => _Attribute.Name;
        public Span<byte> Bytes => _Attribute.Bytes;
        public AttributeDescriptor Descriptor => _Attribute.Descriptor;
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
    }
}
