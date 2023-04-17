using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vim.G3d.Attributes
{
    public static class SubArrayExtensions
    {
        public static IReadOnlyList<int> GetSubArrayCounts(this IReadOnlyList<int> offsets, int maxCount)
        {
            var numItems = offsets?.Count ?? 0;
            var result = new int[numItems];

            if (numItems == 0)
                return result;

            for (var i = 0; i < numItems; ++i)
            {
                result[i] = i < (numItems - 1)
                    ? offsets[i + 1] - offsets[i]
                    : maxCount - offsets[i];
            }

            return result;
        }

        public static IEnumerable<int> GetSubArrayIndices(this IReadOnlyList<int> offsets, IReadOnlyList<int> counts, int index)
        {
            if (offsets == null || offsets.Count == 0 || 
                counts == null || counts.Count == 0)
                return Array.Empty<int>();

            return Enumerable.Range(offsets[index], counts.ElementAtOrDefault(index));
        }
    }
}
