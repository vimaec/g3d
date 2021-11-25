using System;
using Vim.LinqArray;

namespace Vim.G3d
{
    public class G3dScene
    {
        public readonly G3D G3D;
        public readonly int Index;
        public readonly IArray<int> InstanceIndices;
        public readonly IArray<int> ShapeIndices;

        public int SceneInstanceIndexOffset => G3D.SceneIndexOffsets[Index].X;
        public int SceneInstanceCount => G3D.SceneInstanceCounts[Index];

        public int SceneShapeIndexOffset => G3D.SceneIndexOffsets[Index].Y;
        public int SceneShapeCount => G3D.SceneShapeCounts[Index];

        public G3dScene(G3D g3d, int index)
        {
            (G3D, Index) = (g3d, index);
            InstanceIndices = G3D.SceneInstanceIndices?.SubArray(SceneInstanceIndexOffset, SceneInstanceCount) ?? Array.Empty<int>().ToIArray();
            ShapeIndices = G3D.SceneShapeIndices?.SubArray(SceneShapeIndexOffset, SceneShapeCount) ?? Array.Empty<int>().ToIArray();
        }
    }
}
