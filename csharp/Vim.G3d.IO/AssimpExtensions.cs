using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;

namespace Vim.G3d.IO
{
    public static class AssimpExtensions
    {
        public static float[] ToFloats(this IList<Vector3D> vectors)
        {
            var r = new float[vectors.Count * 3];
            for (var i = 0; i < vectors.Count; ++i)
            {
                var v = vectors[i];
                r[i * 3] = v.X;
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
                r[i * 4] = c.R;
                r[i * 4 + 1] = c.G;
                r[i * 4 + 2] = c.B;
                r[i * 4 + 3] = c.A;
            }
            return r;
        }

        public static G3D ToG3D(Mesh mesh)
        {
            var bldr = new G3DBuilder();
            var verts = mesh.Vertices.ToFloats();            
            var indices = mesh.GetIndices();
            var bitangents = mesh.HasTangentBasis ? mesh.BiTangents.ToFloats() : null;
            var tangents = mesh.HasTangentBasis ? mesh.Tangents.ToFloats() : null;
            var normals = mesh.HasNormals ? mesh.Normals.ToFloats() : null;
            var uvChannels = mesh.TextureCoordinateChannels.Take(mesh.TextureCoordinateChannelCount).ToArray();
            var vcChannels = mesh.VertexColorChannels.Take(mesh.VertexColorChannelCount).ToArray();
            var faceSizes = mesh.Faces.Select(f => f.IndexCount).ToArray();
            var faceIndices = new float[faceSizes.Length];
            var index = 0;
            for (var i = 0; i < faceSizes.Length; ++i)
            {
                faceIndices[i] = index;
                index += faceSizes[i];
            }
            throw new NotImplementedException();
        }

        public static Mesh ToAssimpMesh(G3D g3d)
        {

        }
    }
}
