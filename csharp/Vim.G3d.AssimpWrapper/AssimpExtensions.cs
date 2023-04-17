using System;
using System.Linq;
using Assimp;
using Vim.G3d.Attributes;
using Vim.Math3d;

using VimG3d = Vim.G3d.G3d<Vim.G3d.Attributes.VimAttributeCollection>;

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

        public static VimG3d EmptyG3d
            => VimG3d.Empty<VimAttributeCollection>();

        public static VimG3d ToG3d(this Scene scene)
        {
            var g3ds = scene.Meshes.Select(m => m.ToVimG3d()).ToArray();
            var nodes = scene.GetNodes().ToArray();
            if (nodes.Length == 0 || nodes.Length == 1)
                return g3ds.Length > 0 ? g3ds[0] : EmptyG3d;

            return g3ds.Merge();
            //var mergedAttributes = g3ds.Merge().Attributes.ToList();

            //var subGeoTransforms = nodes.Select(n => n.Transform.ToMath3D()).ToInstanceTransformAttribute();
            //mergedAttributes.Add(subGeoTransforms);

            //var meshIndices = nodes.Select(n => n.MeshIndex).ToInstanceMeshAttribute();
            //mergedAttributes.Add(meshIndices);

            //return mergedAttributes.ToG3d();
        }

        public static VimG3d ToVimG3d(this Mesh mesh)
        {
            var g3d = new G3d<VimAttributeCollection>();

            if (mesh.FaceCount == 0)
                return g3d;

            var attributes = g3d.AttributeCollection;

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
            attributes.CornersPerFaceAttribute.TypedData = new[] { numCornersPerFace };

            var indices = mesh.GetIndices();
            if (indices.Length % numCornersPerFace != 0)
                throw new Exception($"The mesh index buffer length {indices.Length} is not divisible by {numCornersPerFace}");

            attributes.IndexAttribute.TypedData = indices;
            attributes.VertexAttribute.TypedData = mesh.Vertices.Select(ToMath3D).ToArray();

            //if (mesh.HasTangentBasis)
            //    bldr.Add(mesh.BiTangents.ToIArray().Select(ToMath3D).ToVertexBitangentAttribute());

            //if (mesh.HasTangentBasis)
            //    bldr.Add(mesh.Tangents.ToIArray().Select(x => ToMath3D(x).ToVector4()).ToVertexTangentAttribute());

            //if (mesh.HasNormals)
            //    bldr.Add(mesh.Normals.ToIArray().Select(ToMath3D).ToVertexNormalAttribute());

            //for (var i = 0; i < mesh.TextureCoordinateChannelCount; ++i)
            //{
            //    var uvChannel = mesh.TextureCoordinateChannels[i];
            //    bldr.Add(uvChannel.ToIArray().Select(ToMath3D).ToVertexUvwAttribute(i));
            //}

            //for (var i = 0; i < mesh.VertexColorChannelCount; ++i)
            //{
            //    var vcChannel = mesh.VertexColorChannels[i];
            //    bldr.Add(vcChannel.ToIArray().Select(ToMath3D).ToVertexColorAttribute(i));
            //}

            return g3d;
        }
    }
}
