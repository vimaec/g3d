using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Vim.G3d
{
    // A very simple .ply file exporter
    public static class G3DPlyExporter
    {
        public static void WritePly(this G3D g, string filePath)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create), Encoding.ASCII);
            g.WritePly(writer);
        }

        public static void WritePly(this G3D g, BinaryWriter writer)
        {
            var vertices = g.Vertices.ToArray();
            var indices = g.Indices.ToArray();
            var faceSize = g.FaceSizes.ToArray();
            var colours = g.VertexColor?.ToArray();
            var normals = g.VertexNormal?.ToArray();
            var uvs = g.UV?.ToArray() ?? g.UVW?.ToArray();
            var uvArity = g.UV != null ? 2 : g.UVW != null ? 3 : 0;

            var numFaces = faceSize != null ? faceSize.Length : (indices.Length / 3);

            //Write the header
            writer.WriteAscii("ply\n");
            writer.WriteAscii("format ascii 1.0\n");
            writer.WriteAscii("element vertex " + vertices.Length / 3 + "\n");
            writer.WriteAscii("property float x\n");
            writer.WriteAscii("property float y\n");
            writer.WriteAscii("property float z\n");
            if (colours != null)
            {
                writer.WriteAscii("property uint8 red\n");
                writer.WriteAscii("property uint8 green\n");
                writer.WriteAscii("property uint8 blue\n");
            }
            if (normals != null)
            {
                writer.WriteAscii("property float nx\n");
                writer.WriteAscii("property float ny\n");
                writer.WriteAscii("property float nz\n");
            }
            if (uvs != null)
            {
                writer.WriteAscii("property float s\n");
                writer.WriteAscii("property float t\n");
            }
            writer.WriteAscii("element face " + numFaces + "\n");
            writer.WriteAscii("property list uchar uint vertex_indices\n");
            writer.WriteAscii("end_header\n");

            // Write the vertices
            for (var i = 0; i < vertices.Length / 3; i++)
            {
                writer.WriteAscii($"{vertices[i * 3 + 0]} {vertices[i * 3 + 1]} {vertices[i * 3 + 2]} ");

                if (colours != null)
                {
                    writer.WriteAscii(
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 0] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 1] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 2] * 255.0f, 255.0f), 0.0f)}");
                }

                if (normals != null)
                {
                    writer.WriteAscii($"{normals[i * 3 + 0]} {normals[i * 3 + 1]} {normals[i * 3 + 2]} ");
                }

                if (uvs != null)
                {
                    writer.WriteAscii($"{uvs[i * uvArity + 0]} {uvs[i * uvArity + 1]} ");
                }

                writer.WriteAscii($"\n");
            }
            

            // Write the face indices
            var index = 0;
            for (var i = 0; i < numFaces; i++)
            {
                var numCorners = faceSize != null ? faceSize[i] : 3;
                writer.WriteAscii(numCorners + " ");
                for (var j = 0; j < numCorners; j++)
                {
                    writer.WriteAscii(indices[index++] + " ");
                }

                writer.WriteAscii("\n");
            }

            //Close the binary writer
            writer.Close();
        }

        private static void WriteAscii(this BinaryWriter writer, string theString)
        {
            var buffer = System.Text.Encoding.ASCII.GetBytes(theString);
            writer.Write(buffer);
        }
    }
}
