using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vim.G3d
{
    public static class ObjExporter
    {
        public static IEnumerable<string> ObjLines(G3D g3d)
        {
            // Write the vertices 
            var vertices = g3d.Vertices.Data.ToArray();
            for (var v=0; v < vertices.Length; v += 3)
                yield return ($"v {vertices[v]} {vertices[v+1]} {vertices[v+2]}");

            var indices = g3d.Indices.Data.ToArray();
            var sb = new StringBuilder();
            var i = 0;
            var f = 0;
            while (i < indices.Length)
            {
                sb.Append("f");
                var faceSize = g3d.FaceSize(f++);
                for (var j = 0; j < faceSize; ++j)
                {
                    var index = g3d.Indices.Data[i++] + 1;
                    sb.Append(" ").Append(index);
                }

                yield return sb.ToString();
                sb.Clear();
            }
        }

        public static void WriteObjFile(this G3D g3d, string filePath)
            => File.WriteAllLines(filePath, ObjLines(g3d));
    }
}