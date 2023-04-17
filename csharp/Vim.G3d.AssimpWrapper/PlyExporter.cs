using System.Collections.Generic;
using System.Text;
using System.IO;
using Vim.G3d.Attributes;

using VimG3d = Vim.G3d.G3d<Vim.G3d.Attributes.VimAttributeCollection>;

namespace Vim.G3d.AssimpWrapper
{
    /// <summary>
    /// A very simple .ply file exporter
    /// </summary> 
    public static class PlyExporter
    {
        public static void WritePly(this VimG3d g, string filePath)
            => File.WriteAllLines(filePath, PlyStrings(g));

        public static IEnumerable<string> PlyStrings(VimG3d g)
        {
            var vertices = g.AttributeCollection.VertexAttribute.TypedData;
            var indices = g.AttributeCollection.IndexAttribute.TypedData;
            var faceCount = g.AttributeCollection.GetFaceCount();
            //var colors = // TODO: restore vertex colors

            //Write the header
            yield return "ply";
            yield return "format ascii 1.0";
            yield return "element vertex " + vertices.Length + "";
            yield return "property float x";
            yield return "property float y";
            yield return "property float z";
            //if (colors != null)
            //{
            //    yield return "property uint8 red";
            //    yield return "property uint8 green";
            //    yield return "property uint8 blue";
            //}
            yield return "element face " + faceCount;
            yield return "property list uint8 int32 vertex_index";
            yield return "end_header";

            // Write the vertices
            //if (colors != null)
            //{
            //    for (var i = 0; i < vertices.Count; i++)
            //    {
            //        var v = vertices[i];
            //        var c = (colors[i] * 255f).Clamp(Vector4.Zero, new Vector4(255, 255, 255, 255));

            //        yield return
            //            $"{v.X} {v.Y} {v.Z} {(byte)c.X} {(byte)c.Y} {(byte)c.Z}";
            //    }
            //}
            //else
            //{
                for (var i = 0; i < vertices.Length; i++)
                {
                    var v = vertices[i];
                    yield return
                        $"{v.X} {v.Y} {v.Z}";
                }
            //}

            // Write the face indices
            var index = 0;
            var sb = new StringBuilder();
            var faceSize = g.AttributeCollection.GetCornersPerFaceCount();
            for (var i = 0; i < faceCount; i++)
            {
                sb.Append(faceSize);
                for (var j = 0; j < faceSize; j++)
                {
                    sb.Append(" ").Append(indices[index++]);
                }

                yield return sb.ToString();
                sb.Clear();
            }
        }
    }
}
