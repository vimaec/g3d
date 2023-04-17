using System;

namespace Vim.G3d
{
    public enum DataType
    {
        /// <summary>
        /// Exists to support future values which are currently unknown.
        /// </summary>
        unknown, 

        // IMPORTANT: the string values of these data types, including their capitalization,
        // are used in the parsing mechanism of the AttributeDescriptor, so be careful!
        uint8,
        int8,
        uint16,
        int16,
        uint32,
        int32,
        uint64,
        int64,
        float32,
        float64,
    };

    public static class DataTypeExtensions
    {
        /// <summary>
        /// Returns the size in bytes of the given data type.
        /// </summary>
        public static int GetDataTypeSize(this DataType dt)
        {
            switch (dt)
            {
                case DataType.uint8:
                case DataType.int8:
                    return 1;
                case DataType.uint16:
                case DataType.int16:
                    return 2;
                case DataType.uint32:
                case DataType.int32:
                    return 4;
                case DataType.uint64:
                case DataType.int64:
                    return 8;
                case DataType.float32:
                    return 4;
                case DataType.float64:
                    return 8;
                case DataType.unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }

        public static Type GetManagedType(this DataType dt)
        {
            switch (dt)
            {
                case DataType.uint8:
                    return typeof(byte);
                case DataType.int8:
                    return typeof(sbyte);
                case DataType.uint16:
                    return typeof(ushort);
                case DataType.int16:
                    return typeof(short);
                case DataType.uint32:
                    return typeof(uint);
                case DataType.int32:
                    return typeof(int);
                case DataType.uint64:
                    return typeof(ulong);
                case DataType.int64:
                    return typeof(long);
                case DataType.float32:
                    return typeof(float);
                case DataType.float64:
                    return typeof(double);
                case DataType.unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }
    }
}
