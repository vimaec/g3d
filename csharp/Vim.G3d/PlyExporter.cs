using System.Collections.Generic;
using System.Text;
using System.IO;
using Vim.Math3d;

namespace Vim.G3d
{
    /// <summary>
    /// A very simple .ply file exporter
    /// </summary> 
    public static class PlyExporter
    {
        public static void WritePly(this G3D g, string filePath)
            => File.WriteAllLines(filePath, PlyStrings(g));

        public static IEnumerable<string> PlyStrings(G3D g)
        {
            var vertices = g.Vertices;
            var indices = g.Indices;
            var colors = g.VertexColors;

            //Write the header
            yield return "ply";
            yield return "format ascii 1.0";
            yield return "element vertex " + vertices.Count + "";
            yield return "property float x";
            yield return "property float y";
            yield return "property float z";
            if (colors != null)
            {
                yield return "property uint8 red";
                yield return "property uint8 green";
                yield return "property uint8 blue";
            }
            yield return "element face " + g.NumFaces;
            yield return "property list uint8 int32 vertex_index";
            yield return "end_header";

            // Write the vertices
            if (colors != null)
            {
                for (var i = 0; i < vertices.Count; i++)
                {
                    var v = vertices[i];
                    var c = (colors[i] * 255f).Clamp(Vector4.Zero, new Vector4(255,255,255,255));

                    yield return
                        $"{v.X} {v.Y} {v.Z} {(byte) c.X} {(byte) c.Y} {(byte) c.Z}";
                }
            }
            else
            {
                for (var i = 0; i < vertices.Count; i++)
                {
                    var v = vertices[i];
                    yield return
                        $"{v.X} {v.Y} {v.Z}";
                }
            }

            // Write the face indices
            var index = 0;
            var sb = new StringBuilder();
            var faceSize = g.NumCornersPerFace;
            for (var i = 0; i < g.NumFaces; i++)
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
