using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vim.G3d
{
    public interface IAttribute
    {
        string Name { get; }
        IAttributeDescriptor AttributeDescriptor { get; }
        AttributeType AttributeType { get; }
        Type IndexInto { get; }
        Array Data { get; }
        void Write(Stream stream);
    }

    public interface IAttribute<T> : IAttribute
    {
        T[] TypedData { get; set; }
    }

    public static class AttributeExtensions
    {
        public static long GetSizeInBytes(this IAttribute attribute)
            => attribute.AttributeDescriptor.DataElementSize * (attribute.Data?.Length ?? 0);

        public static TAttr MergeDataAttributes<TAttr, U>(this IReadOnlyList<TAttr> attributes) where TAttr : IAttribute<U>, new()
        {
            if (attributes.Count == 0)
                return new TAttr();

            // Check that all attributes have the same descriptor 
            var first = attributes.First();
            if (!attributes.All(attr => attr.Name.Equals(first.Name)))
                throw new Exception($"All attributes must have the same descriptor ({first.Name}) to be merged.");

            var data = attributes.SelectMany(attr => attr.TypedData).ToArray();
            return new TAttr { TypedData = data };
        }

        public static TAttr MergeIndexAttributes<TAttr>(
            this IEnumerable<(TAttr Attribute, int IndexedCount)> toMerge)
            where TAttr : IAttribute<int>, new()
        {
            var mergeList = toMerge.ToArray();
            var mergedIndices = new int[mergeList.Sum(t => t.Attribute.TypedData.Length)];

            var valueOffset = 0;
            var mergedCount = 0;

            foreach (var (attr, indexedCount) in toMerge)
            {
                var typedData = attr.TypedData;
                var typedDataCount = typedData.Length;

                for (var i = 0; i < typedDataCount; ++i)
                {
                    mergedIndices[mergedCount + i] = typedData[i] < 0
                        ? typedData[i] // if the index is less than 0, then do not increment it (this preserves -1 value assignments)
                        : typedData[i] + valueOffset; // otherwise, apply the increment.
                }

                mergedCount += typedDataCount;
                valueOffset += indexedCount;
            }

            return new TAttr { TypedData = mergedIndices };
        }
    }
}
