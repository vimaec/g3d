using System.Collections.Generic;
using System.Text;
using System.IO;
using Vim.G3d.Attributes;

using VimG3d = Vim.G3d.G3d<Vim.G3d.Attributes.VimAttributeCollection>;

namespace Vim.G3d.AssimpWrapper
{
    /// <summary>
    /// This is a simple ObjExporter for the purposes of testing.
    /// </summary>
    public static class ObjExporter
    {
        public static IEnumerable<string> ObjLines(VimG3d g3d)
        {
            // Write the vertices 
            var vertices = g3d.AttributeCollection.VertexAttribute.TypedData;
            //var uvs = TODO: restore support for UVs in VimG3d;
            foreach (var v in vertices)
                yield return ($"v {v.X} {v.Y} {v.Z}");
            //if (uvs != null)
            //{
            //    for (var v = 0; v < uvs.Count; v++)
            //        yield return ($"vt {uvs[v].X} {uvs[v].Y}");
            //}

            var indices = g3d.AttributeCollection.IndexAttribute.TypedData;
            var sb = new StringBuilder();
            var i = 0;
            var faceSize = g3d.AttributeCollection.GetCornersPerFaceCount();
            while (i < indices.Length)
            {
                sb.Append("f");

                //if (uvs == null)
                //{
                    for (var j = 0; j < faceSize; ++j)
                    {
                        var index = indices[i++] + 1;
                        sb.Append(" ").Append(index);
                    }
                //}
                //else
                //{
                //    for (var j = 0; j < faceSize; ++j)
                //    {
                //        var index = g3d.Indices[i++] + 1;
                //        sb.Append(" ").Append(index).Append("/").Append(index);
                //    }
                //}

                yield return sb.ToString();
                sb.Clear();
            }
        }

        public static void WriteObj(this VimG3d g3d, string filePath)
            => File.WriteAllLines(filePath, ObjLines(g3d));
    }
}