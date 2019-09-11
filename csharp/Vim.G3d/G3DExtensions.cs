using System.Collections.Generic;
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
    }
}
