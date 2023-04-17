using System;
using System.Collections.Generic;
using System.Text;

// TODO: WORK IN PROGRESS

namespace Vim.G3d.Attributes
{
    public class VimG3dShapeFactory
    {
        private readonly VimAttributeCollection _vac;
        private readonly IReadOnlyList<int> _shapeVertexCounts;

        public VimG3dShapeFactory(VimAttributeCollection vac)
        {
            _vac = vac;
            _shapeVertexCounts = _vac.ShapeVertexOffsetAttribute?.TypedData.GetSubArrayCounts(_vac.GetSubmeshCount());
        }

        public IEnumerable<int> GetShapeVertexIndices(int shapeIndex)
            => _vac.ShapeVertexOffsetAttribute?.TypedData.GetSubArrayIndices(_shapeVertexCounts, shapeIndex);

        // TODO: create shapes
    }
}
