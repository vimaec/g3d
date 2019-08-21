using System;
using System.Collections.Generic;
using System.Text;

namespace Vim.G3d
{
    public class G3DBuilder
    {
        public Header Header { get; set; }
        public readonly List<Attribute> Attributes = new List<Attribute>();

        public void AddAttribute(Attribute attr)
            => Attributes.Add(attr);

        public void AddIndices(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.Indices);

        public void AddVertices(float[] vertices)
            => AddAttribute()
    }
}
