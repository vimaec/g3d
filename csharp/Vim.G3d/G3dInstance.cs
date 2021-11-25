using Vim.Math3d;

namespace Vim.G3d
{
    public class G3dInstance
    {
        public readonly G3D G3D;
        public readonly int Index;

        public int ParentIndex => G3D.InstanceParents[Index];
        public int MeshIndex => G3D.InstanceMeshes[Index];
        public Matrix4x4 Transform => G3D.InstanceTransforms[Index];

        public G3dInstance(G3D g3d, int index)
        {
            (G3D, Index) = (g3d, index);
        }
    }
}
