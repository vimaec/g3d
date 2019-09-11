using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Vim.G3d
{
    /// <summary>
    /// A very simple .ply file exporter
    /// </summary> 
    public static class PlyExporter
    {
        public static void WritePlyFile(this G3D g, string filePath)
            => File.WriteAllLines(filePath, PlyStrings(g));

        public static IEnumerable<string> PlyStrings(G3D g)
        {
            var vertices = g.Vertices.ToArray();
            var indices = g.Indices.ToArray();
            var colors = g.VertexColor.ElementAtOrDefault(0)?.ToArray();

            //Write the header
            yield return "ply;";
            yield return "format ascii 1.0";
            yield return "element vertex " + vertices.Length / 3 + "";
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
                for (var i = 0; i < vertices.Length / 3; i++)
                {
                    yield return 
                        $"{vertices[i * 3 + 0]} {vertices[i * 3 + 1]} {vertices[i * 3 + 2]} " +
                        $"{(byte)Math.Max(Math.Min(colors[i * 3 + 0] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colors[i * 3 + 1] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colors[i * 3 + 2] * 255.0f, 255.0f), 0.0f)}";
                }
            }
            else
            {
                for (var i = 0; i < vertices.Length / 3; i++)
                {
                    yield return $"{vertices[i * 3 + 0]} {vertices[i * 3 + 1]} {vertices[i * 3 + 2]}";
                }
            }

            // Write the face indices
            var index = 0;
            var sb = new StringBuilder();
            for (var i = 0; i < g.NumFaces; i++)
            {
                var faceSize = g.FaceSize(i);
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
