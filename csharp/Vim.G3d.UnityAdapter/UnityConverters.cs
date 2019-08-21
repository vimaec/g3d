using System;
using UnityEngine;

namespace Vim.G3d.Unity
{
    /*
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
            mesh.uv2 = g3d.UVs[1].ToArray();
            mesh.uv3 = g3d.UVs[2].ToArray();
            mesh.uv4 = g3d.UVs[3].ToArray();
            mesh.uv5 = g3d.UVs[4].ToArray();
            mesh.uv6 = g3d.UVs[5].ToArray();
            mesh.uv7 = g3d.UVs[6].ToArray();
            mesh.uv8 = g3d.UVs[7].ToArray();
            return mesh;
        }

        public static UnityAttributes ToAttributes(this Mesh mesh)
        {
            var g3d = new G3D();
            g3d.Position = mesh.vertices = g3d.Position.ToArray();
            mesh.triangles = g3d.Indices.ToArray();
            mesh.colors = g3d.Colors.ToArray();
            mesh.normals = g3d.Normals.ToArray();
            mesh.tangents = g3d.Tangents.ToArray();
            mesh.uv = g3d.UVs[0].ToArray();
            mesh.uv2 = g3d.UVs[1].ToArray();
            mesh.uv3 = g3d.UVs[2].ToArray();
            mesh.uv4 = g3d.UVs[3].ToArray();
            mesh.uv5 = g3d.UVs[4].ToArray();
            mesh.uv6 = g3d.UVs[5].ToArray();
            mesh.uv7 = g3d.UVs[6].ToArray();
            mesh.uv8 = g3d.UVs[7].ToArray();
            return mesh;
        }
    }
    */
}
