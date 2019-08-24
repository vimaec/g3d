using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;

namespace Vim.G3d
{
    public static class AssimpExtensions
    {
        public static float[] ToFloats(this IList<Vector3D> vectors)
        {
            var r = new float[vectors.Count * 3];
            for (var i = 0; i < vectors.Count; ++i)
            {
                var v = vectors[i];
                r[i * 3 + 0] = v.X;
                r[i * 3 + 1] = v.Y;
                r[i * 3 + 2] = v.Z;
            }
            return r;
        }

        public static float[] ToFloats(this IList<Color4D> colors)
        {
            var r = new float[colors.Count * 4];
            for (var i = 0; i < colors.Count; ++i)
            {
                var c = colors[i];
                r[i * 4 + 0] = c.R;
                r[i * 4 + 1] = c.G;
                r[i * 4 + 2] = c.B;
                r[i * 4 + 3] = c.A;
            }
            return r;
        }

        public static bool IsTriangularMesh(Mesh mesh)
            => mesh.Faces.All(f => f.IndexCount == 3);

        public static G3D ToG3D(this Mesh mesh)
        {
            var bldr = new G3DBuilder();

            bldr.AddVertices(mesh.Vertices.ToFloats());

            // Is it triangular or polygonal 
            if (IsTriangularMesh(mesh))
                bldr.AddIndices(mesh.GetIndices());
            else
                bldr.AddIndicesByFace(mesh.Faces.Select((f => f.Indices)));

            if (mesh.HasTangentBasis)
                bldr.AddBitangent(mesh.BiTangents.ToFloats());

            if (mesh.HasTangentBasis)
                bldr.AddTangent(mesh.Tangents.ToFloats());

            if (mesh.HasNormals)
                bldr.AddVertexNormals(mesh.Normals.ToFloats());

            for (var i = 0; i < mesh.TextureCoordinateChannelCount; ++i)
            {
                var uvChannel = mesh.TextureCoordinateChannels[i];
                bldr.AddUVW(uvChannel.ToFloats());
            }

            for (var i = 0; i < mesh.VertexColorChannelCount; ++i)
            {
                var vcChannel = mesh.VertexColorChannels[i];
                bldr.AddVertexColorsWithAlpha(vcChannel.ToFloats());
            }

            return bldr.ToG3D();
        }
    }
}
