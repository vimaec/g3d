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
            var vertices = g.Vertices.ToArray();
            var indices = g.Indices.ToArray();
            var faceSize = g.FaceSizes.ToArray();
            var colours = g.Color?.ToArray();

            BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create), Encoding.ASCII);

            //Write the header
            writer.Write(StringToByteArray("ply\n"));
            writer.Write(StringToByteArray("format ascii 1.0\n"));
            writer.Write(StringToByteArray("element vertex " + vertices.Length / 3 + "\n"));
            writer.Write(StringToByteArray("property float x\n"));
            writer.Write(StringToByteArray("property float y\n"));
            writer.Write(StringToByteArray("property float z\n"));
            if (colours != null)
            {
                writer.Write(StringToByteArray("property uint8 red\n"));
                writer.Write(StringToByteArray("property uint8 green\n"));
                writer.Write(StringToByteArray("property uint8 blue\n"));
            }
            writer.Write(StringToByteArray("element face " + faceSize.Length + "\n"));
            writer.Write(StringToByteArray("property list uint8 int32 vertex_indices\n"));
            writer.Write(StringToByteArray("end_header\n"));

            // Write the vertices
            if (colours != null)
            {
                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    writer.Write(StringToByteArray(
                        vertices[i * 3 + 0] + " " +
                        vertices[i * 3 + 1] + " " +
                        vertices[i * 3 + 2] + " " +
                        (byte)Math.Max(Math.Min(colours[i * 3 + 0] * 255.0f, 255.0f), 0.0f) + " " +
                        (byte)Math.Max(Math.Min(colours[i * 3 + 1] * 255.0f, 255.0f), 0.0f) + " " +
                        (byte)Math.Max(Math.Min(colours[i * 3 + 2] * 255.0f, 255.0f), 0.0f) + "\n"));
                }
            }
            else
            {
                for (int i = 0; i < vertices.Length / 3; i++)
                {
                    writer.Write(StringToByteArray(
                        vertices[i * 3 + 0] + " " +
                        vertices[i * 3 + 1] + " " +
                        vertices[i * 3 + 2] + "\n"));
                }
            }

            // Write the face indices
            int index = 0;
            for (int i = 0; i < faceSize.Length; i++)
            {
                writer.Write(StringToByteArray(faceSize[i] + " "));
                for (int j = 0; j < faceSize[i]; j++)
                {
                    writer.Write(StringToByteArray(indices[index++] + " "));
                }

                writer.Write(StringToByteArray("\n"));
            }

            //Close the binary writer
            writer.Close();
        }

        private static byte[] StringToByteArray(string theString)
        {
            return System.Text.Encoding.ASCII.GetBytes(theString);
        }
    }
}
