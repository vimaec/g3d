using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d
{
    public static class AttributeExtensions
    {
        public static GeometryAttribute<T> CheckArity<T>(this GeometryAttribute<T> self, int arity) where T : unmanaged
            => self?.Descriptor?.DataArity == arity ? self : null;

        public static GeometryAttribute<T> CheckAssociation<T>(this GeometryAttribute<T> self, Association assoc) where T : unmanaged
            => self?.Descriptor?.Association == assoc ? self : null;

        public static GeometryAttribute<T> CheckArityAndAssociation<T>(this GeometryAttribute<T> self, int arity, Association assoc) where T : unmanaged
            => self?.CheckArity(arity)?.CheckAssociation(assoc);

        public static GeometryAttribute<T> ToAttribute<T>(this IList<T> self, string desc) where T : unmanaged
            => self.ToIArray().ToAttribute(desc);

        public static GeometryAttribute<T> ToAttribute<T>(this IList<T> self, AttributeDescriptor desc) where T : unmanaged
            => self.ToIArray().ToAttribute(desc);

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, AttributeDescriptor desc) where T : unmanaged
            => new GeometryAttribute<T>(self, desc);

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, string desc) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc));

        public static GeometryAttribute<T> ToAttribute<T>(this IArray<T> self, string desc, int index) where T : unmanaged
            => self.ToAttribute(AttributeDescriptor.Parse(desc).SetIndex(index));

        public static IArray<Vector4> AttributeToColors(this GeometryAttribute attr)
        {
            var desc = attr.Descriptor;
            if (desc.DataType == DataType.dt_float32)
            {
                if (desc.DataArity == 4)
                    return attr.AsType<Vector4>().Data;
                if (desc.DataArity == 3)
                    return attr.AsType<Vector3>().Data.Select(vc => new Vector4(vc, 1f));
                if (desc.DataArity == 2)
                    return attr.AsType<Vector2>().Data.Select(vc => new Vector4(vc.X, vc.Y, 0, 1f));
                if (desc.DataArity == 1)
                    return attr.AsType<float>().Data.Select(vc => new Vector4(vc,vc,vc, 1f));
            }
            if (desc.DataType == DataType.dt_int8)
            {
                if (desc.DataArity == 4)
                    return attr.AsType<Byte4>().Data.Select(b => new Vector4((float)b.X / 255f, (float)b.Y / 255f, (float)b.Z / 255f, (float)b.W / 255f));
                if (desc.DataArity == 3)
                    return attr.AsType<Byte3>().Data.Select(b => new Vector4((float)b.X / 255f, (float)b.Y / 255f, (float)b.Z / 255f, 1f));
                if (desc.DataArity == 2)
                    return attr.AsType<Byte2>().Data.Select(b => new Vector4((float)b.X / 255f, (float)b.Y / 255f, 0f, 1f));
                if (desc.DataArity == 1)
                    return attr.AsType<byte>().Data.Select(b => new Vector4((float)b / 255f, (float)b / 255f, (float)b / 255f, 1f));
            }
            Debug.WriteLine($"Failed to recongize color format {attr.Descriptor}");
            return null;
        }

        public static GeometryAttribute ToDefaultAttribute(this AttributeDescriptor desc, int count)
        {
            switch (desc.DataType)
            {
                case DataType.dt_int8:
                    if (desc.DataArity == 1)
                        return default(byte).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(Byte2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(Byte3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(Byte4).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int16:
                    if (desc.DataArity == 1)
                        return default(short).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int32:
                    if (desc.DataArity == 1)
                        return default(int).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(Int2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(Int3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(Int4).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_int64:
                    if (desc.DataArity == 1)
                        return default(long).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_float32:
                    if (desc.DataArity == 1)
                        return default(float).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(Vector2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(Vector3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(Vector4).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 16)
                        return default(Matrix4x4).Repeat(count).ToAttribute(desc);
                    break;
                case DataType.dt_float64:
                    if (desc.DataArity == 1)
                        return default(double).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 2)
                        return default(DVector2).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 3)
                        return default(DVector3).Repeat(count).ToAttribute(desc);
                    if (desc.DataArity == 4)
                        return default(DVector4).Repeat(count).ToAttribute(desc);
                    break;
            }

            throw new Exception($"Could not create a default attribute for {desc}");
        }

        public static long GetByteSize(this GeometryAttribute attribute)
            => (long)attribute.ElementCount * attribute.Descriptor.DataElementSize;

        public static GeometryAttribute Merge(this IEnumerable<GeometryAttribute> attributes)
            => attributes.FirstOrDefault()?.Merge(attributes.Skip(1));

        public static GeometryAttribute Merge(this IArray<GeometryAttribute> attributes)
            => attributes.ToEnumerable().Merge();
    }
}
