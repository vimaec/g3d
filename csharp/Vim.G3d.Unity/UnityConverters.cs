using System;
using UnityEngine;

namespace Vim.G3d.Unity
{
    public class UnityAttributes
    {
        public Attribute<Vector3> Position;
        public Attribute<int> Indices;
        public Attribute<Vector3> Normals;
        public Attribute<Vector4> Tangents;
        public Attribute<Color> Colors;
        public Attribute<int> SubMeshIndices;
        public Attribute<int> SubMeshSizes;
        public Attribute<int> SubMeshVertexOffsets;
        public Attribute<Vector2>[] UVs = new Attribute<Vector2>[8];

        public UnityAttributes(G3D g3d)
        {
            // TODO: fill out each attribute from the G3D object
        }
    }

    public static class UnityConverters
    {
        public static Mesh CopyToUnityMesh(this UnityAttributes g3d, Mesh mesh)
        {
            mesh.vertices = g3d.Position.ToArray();
            mesh.triangles = g3d.Indices.ToArray();
            mesh.colors = g3d.Colors.ToArray();
            mesh.normals = g3d.Normals.ToArray();
            mesh.tangents = g3d.Tangents.ToArray();
            mesh.uv = g3d.UVs[0].ToArray();
            return mesh;
        }
    }
}
