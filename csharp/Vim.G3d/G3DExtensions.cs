using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vim.G3d
{
    public static class G3DExtension
    {
        public static G3D ToG3D(this IEnumerable<BinaryAttribute> attributes, Header header = null)
            => new G3D(attributes, header);

        public static G3D ToG3D(this IList<INamedBuffer> buffers)
            => new G3D(buffers.Skip(1).Select(AttributeExtensions.ToAttribute), new Header(buffers[0].GetString()));

        public static int FaceSize(this G3D g3d, int n)
            => g3d.FaceSizes?.Data[n] ?? g3d.CornersPerFace;

        public static IEnumerable<INamedBuffer> ToBuffers(this G3D self)
            => new[] { self.Header.ToString().ToNamedBuffer("meta") } // First buffer is named "meta"
            .Concat(self.Attributes); // All other attributes are subsequent buffers 

        public static byte[] ToBytes(this G3D self)
            => self.ToBuffers().Pack();

        public static void Write(this G3D self, Stream stream)
            => BFast.WriteBFast(self.ToBuffers(), stream);

        public static void Write(this G3D self, string filePath)
            => self.Write(File.OpenWrite(filePath));

        public static G3D ToG3D(this byte[] bytes)
            => bytes.Unpack().ToG3D();

        public static G3D ToG3D(this INamedBuffer buffer)
            => buffer.Bytes.ToArray().ToG3D();
    }
}
