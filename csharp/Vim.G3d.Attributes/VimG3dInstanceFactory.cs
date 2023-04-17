using System.Collections.Generic;

// TODO: WORK IN PROGRESS.

namespace Vim.G3d.Attributes
{
    public class VimG3dInstanceFactory
    {
        private readonly VimAttributeCollection _vac;
        private readonly IReadOnlyList<int> _meshSubmeshCount;

        public VimG3dInstanceFactory(VimAttributeCollection vac)
        {
            _vac = vac;
            _meshSubmeshCount = _vac.MeshSubmeshOffsetAttribute?.TypedData.GetSubArrayCounts(_vac.GetSubmeshCount());
        }

        public IEnumerable<int> GetMeshSubmeshIndices(int meshIndex)
            => _vac.MeshSubmeshOffsetAttribute?.TypedData.GetSubArrayIndices(_meshSubmeshCount, meshIndex);

        // TODO: create instances
    }
}
