using System;
using System.IO;
using System.Linq;
using Vim.BFast;
using Vim.LinqArray;

namespace Vim.G3d
{
    public static partial class G3DExtension
    {
        public static void WriteAttribute(Stream stream, GeometryAttribute attribute, string name, long size)
        {
            var buffer = attribute.ToBuffer();
            if (buffer.NumBytes() != size)
                throw new Exception($"Internal error while writing attribute, expected number of bytes was {size} but instead was {buffer.NumBytes()}");
            if (buffer.Name != name)
                throw new Exception($"Internal error while writing attribute, expected name was {name} but instead was {buffer.Name}");
            stream.Write(buffer);
        }

        public static G3dWriter ToG3DWriter(this IGeometryAttributes self, G3dHeader? header = null)
            => new G3dWriter(self, header);

        public static void Write(this IGeometryAttributes self, Stream stream, G3dHeader? header = null)
            => self.ToG3DWriter(header).Write(stream);

        public static void Write(this IGeometryAttributes self, string filePath, G3dHeader? header = null)
        {
            using (var stream = File.OpenWrite(filePath))
                self.Write(stream, header);
        }

        public static byte[] WriteToBytes(this IGeometryAttributes self)
        {
            using (var memoryStream = new MemoryStream())
            {
                self.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static G3D ReadG3d(this Stream stream)
        {
            var header = G3dHeader.Default;

            GeometryAttribute ReadAttribute(Stream s2, string name, long size)
            {
                // Check for the G3dHeader 
                if (name == "meta")
                {
                    if (size == 8)
                    {
                        var buffer = s2.ReadArray<byte>((int)size);
                        if (buffer[0] == G3dHeader.MagicA && buffer[1] == G3dHeader.MagicB)
                            header = G3dHeader.FromBytes(buffer);
                    }
                    else
                    {
                        s2.ReadArray<byte>((int)size);
                    }
                    return null;
                }
                else
                {
                    // Figure out the correct type and then read it in 
                    var desc = AttributeDescriptor.Parse(name);
                    var dflt = desc.ToDefaultAttribute(0);
                    return dflt.Read(stream, size);
                }
            }

            var results = stream.ReadBFast(ReadAttribute).Select(r => r.Item2);
            return new G3D(results.Where(x => x != null), header);
        }

    }
}
