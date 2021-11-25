using System;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d
{
    public class G3dShape
    {
        public readonly G3D G3D;
        public readonly int Index;
        public readonly IArray<Vector3> Vertices;

        public int ShapeVertexOffset => G3D.ShapeVertexOffsets[Index];
        public int ShapeVertexCount => G3D.ShapeVertexCounts[Index];
        public Vector4 Color => G3D.ShapeColors[Index];
        public float Width => G3D.ShapeWidths[Index];

        public G3dShape(G3D g3d, int index)
        {
            (G3D, Index) = (g3d, index);
            Vertices = G3D.ShapeVertices?.SubArray(ShapeVertexOffset, ShapeVertexCount) ?? Array.Empty<Vector3>().ToIArray();
        }
    }
}
