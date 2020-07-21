using System;
using System.Linq;
using Assimp;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d.AssimpWrapper
{
    public static class AssimpExtensions
    {
        public static Vector2 ToMath3D(this Vector2D v)
            => new Vector2(v.X, v.Y);

        public static Vector3 ToMath3D(this Vector3D v)
            => new Vector3(v.X, v.Y, v.Z);

        public static Vector3 ToMath3D(this Color3D v)
            => new Vector3(v.R, v.G, v.B);

        public static Vector4 ToMath3D(this Color4D v)
            => new Vector4(v.R, v.G, v.B, v.A);

        public static Math3d.Matrix4x4 ToMath3D(this Assimp.Matrix4x4 m)
            => new Math3d.Matrix4x4(
                m.A1, m.A2, m.A3, m.A4,
                m.B1, m.B2, m.B3, m.B4,
                m.C1, m.C2, m.C3, m.C4,
                m.D1, m.D2, m.D3, m.D4);

        public static bool IsTriangular(this Mesh mesh)
            => mesh.Faces.All(f => f.IndexCount == 3);

        public static G3D ToG3d(this Scene scene)
        {
            var meshes = scene.Meshes.Select(m => m.ToG3D()).ToIArray();
            var nodes = scene.GetNodes().ToIArray();
            if (nodes.Count == 0 || nodes.Count == 1)
                return meshes.Count > 0 ? meshes[0] : G3D.Empty;

            var mergedAttributes = meshes.Merge().Attributes.ToList();

            var subGeoTransforms = nodes.Select(n => n.Transform.ToMath3D()).ToInstanceTransformsAttribute();
            mergedAttributes.Add(subGeoTransforms);

            var subGeoIndices = nodes.Select(n => n.MeshIndex).ToInstanceGeometriesAttribute();
            mergedAttributes.Add(subGeoIndices);

            return mergedAttributes.ToG3d();
        }

        public static G3D ToG3D(this Mesh mesh)
        {
            // The Assimp mesh must be triangulated
            if (mesh.FaceCount == 0)
                return G3D.Empty;

            var bldr = new G3DBuilder();

            // Note: should be at most 3 for meses, but could 2 for lines, or 1 for point clouds
            var numCornersPerFace = mesh.Faces[0].IndexCount;
            if (numCornersPerFace > 3)
                throw new Exception("The Assimp mesh must be triangulated as a post-process");
            if (numCornersPerFace <= 0)
                throw new Exception("The Assimp mesh has faces without indices");
            foreach (var f in mesh.Faces)
            {
                if (f.IndexCount != numCornersPerFace)
                    throw new Exception($"Each face of the assimp mesh must have {numCornersPerFace} corners, but found one with {f.IndexCount}");
            }
            bldr.SetObjectFaceSize(numCornersPerFace);

            var indices = mesh.GetIndices();
            if (indices.Length % numCornersPerFace != 0)
                throw new Exception($"The mesh index buffer length {indices.Length} is not divisible by {numCornersPerFace}");

            bldr.AddVertices(mesh.Vertices.ToIArray().Select(ToMath3D));
            bldr.AddIndices(indices);

            if (mesh.HasTangentBasis)
                bldr.Add(mesh.BiTangents.ToIArray().Select(ToMath3D).ToVertexBitangentAttribute());

            if (mesh.HasTangentBasis)
                bldr.Add(mesh.Tangents.ToIArray().Select(x => ToMath3D(x).ToVector4()).ToVertexTangentAttribute());

            if (mesh.HasNormals)
                bldr.Add(mesh.Normals.ToIArray().Select(ToMath3D).ToVertexNormalAttribute());

            for (var i = 0; i < mesh.TextureCoordinateChannelCount; ++i)
            {
                var uvChannel = mesh.TextureCoordinateChannels[i];
                bldr.Add(uvChannel.ToIArray().Select(ToMath3D).ToVertexUvwAttribute(i));
            }

            for (var i = 0; i < mesh.VertexColorChannelCount; ++i)
            {
                var vcChannel = mesh.VertexColorChannels[i];
                bldr.Add(vcChannel.ToIArray().Select(ToMath3D).ToVertexColorAttribute(i));
            }

            return bldr.ToG3D();
        }
    }
}
