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
            writer.WriteAscii("element face " + faceSize.Length + "\n");
            writer.WriteAscii("property list uint8 int32 vertex_indices\n");
            writer.WriteAscii("end_header\n");

            // Write the vertices
            if (colours != null)
            {
                for (var i = 0; i < vertices.Length / 3; i++)
                {
                    writer.WriteAscii($"{vertices[i * 3 + 0]} {vertices[i * 3 + 1]} {vertices[i * 3 + 2]} ");
                    writer.WriteAscii(
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 0] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 1] * 255.0f, 255.0f), 0.0f)} " +
                        $"{(byte)Math.Max(Math.Min(colours[i * 3 + 2] * 255.0f, 255.0f), 0.0f)}\n\n");
                }
            }
            else
            {
                for (var i = 0; i < vertices.Length / 3; i++)
                {
                    writer.WriteAscii($"{vertices[i * 3 + 0]} {vertices[i * 3 + 1]} {vertices[i * 3 + 2]}\n");
                }
            }

            // Write the face indices
            var index = 0;
            for (var i = 0; i < faceSize.Length; i++)
            {
                writer.WriteAscii(faceSize[i] + " ");
                for (var j = 0; j < faceSize[i]; j++)
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
