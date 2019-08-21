using System.Collections.Generic;

namespace Vim.G3d
{
    public static class G3DExtension
    {
        public static G3D ToG3D(this IEnumerable<BinaryAttribute> attributes, Header header = null)
            => new G3D(attributes, header);
    }
}
