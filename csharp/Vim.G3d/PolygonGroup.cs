using System;

namespace Vim.G3d
{
    /// <summary>
    /// A polygon group is a contiguous series of faces in the mesh. 
    /// </summary>
    public class PolygonGroup
    {
        public int BeginIndex { get; }
        public int EndIndex { get; }
        public int IndexCount => EndIndex - BeginIndex;
        public int CornersPerFace { get; }
        public int MaterialId { get; }
        public int Id { get; }
        public Span<int> Indices => G3D.Indices.Data.Slice(BeginIndex, IndexCount);

        public G3D G3D { get; }

        public PolygonGroup(G3D g, int i)
        {
            if (g.NumGroups <= 0)
                throw new Exception("No polygon groups present in G3D");

            var fi = g.GroupIndexOffsets?.CheckArityAndAssociation(1, Association.assoc_group);
            if (fi == null)
                throw new Exception("Polygon groups missing index offsets");
            BeginIndex = fi.Data[i];
            EndIndex = (i >= fi.ElementCount - 1) ? g.Indices.ElementCount : fi.Data[i + 1];

            G3D = g;
            Id = i;
            if (g.CornersPerFace > 0)
            {
                CornersPerFace = g.CornersPerFace;
            }
            else
            {
                var fs = g.FaceSizes?.CheckArityAndAssociation(1, Association.assoc_group);
                if (fs != null)
                    CornersPerFace = fs.Data[i];
            }

            var mi = g.MaterialIds?.CheckArityAndAssociation(1, Association.assoc_group);
            if (mi != null)
                MaterialId = mi.Data[i];
        }
    }
}