using System;
using System.IO;
using System.Text;
using Vim.BFast;

namespace Vim.G3d
{
    // http://docs.autodesk.com/FBX/2014/ENU/FBX-SDK-Documentation/index.html?url=files/GUID-CC93340E-C4A1-49EE-B048-E898F856CFBF.htm,topicNumber=d30e8478
    // https://twitter.com/FreyaHolmer/status/644881436982575104
    // https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#coordinate-system-and-units

    public struct MetaHeader
    {
        public byte MagicA; // 0x63
        public byte MagicB; // 0xD0
        public byte UnitA; // with unitB could be: 'ft', 'yd', 'mi', 'km', 'mm', 'in', 'cm', 'm',
        public byte UnitB;
        public byte UpAxis; // e.g. 1=y or 2=z (could be 0=x, if you hate people)
        public byte ForwardVector; // e.g. 0=x, 1=y, 2=z, 3=-x, 4=-y, 5=-z
        public byte Handedness; // 0=left-handed, 1=right-handed
        public byte Padding; // 0

        public string Unit => Encoding.ASCII.GetString(new byte[] { UnitA, UnitB });

        public byte[] ToBytes()
            => new[] { MagicA, MagicB, UnitA, UnitB, UpAxis, ForwardVector, Handedness, Padding };

        public NamedBuffer<byte> ToNamedBuffer()
            => ToBytes().ToNamedBuffer(Constants.MetaHeaderSegmentName);

        public static MetaHeader FromBytes(byte[] bytes)
            => new MetaHeader
            {
                MagicA = bytes[0],
                MagicB = bytes[1],
                UnitA = bytes[2],
                UnitB = bytes[3],
                UpAxis = bytes[4],
                ForwardVector = bytes[5],
                Handedness = bytes[6],
            }
            .Validate();

        public static MetaHeader Default
            = new MetaHeader
            {
                MagicA = Constants.MetaHeaderMagicA,
                MagicB = Constants.MetaHeaderMagicB,
                UnitA = (byte)'m',
                UnitB = 0,
                UpAxis = 2,
                ForwardVector = 0,
                Handedness = 0,
                Padding = 0
            };

        public MetaHeader Validate()
        {
            if (MagicA != Constants.MetaHeaderMagicA) throw new Exception($"First magic number must be 0x{Constants.MetaHeaderMagicA:X2} not 0x{MagicA:X2}");
            if (MagicB != Constants.MetaHeaderMagicB) throw new Exception($"Second magic number must be 0x{Constants.MetaHeaderMagicB:X2} not 0x{MagicB:X2}");
            if (Array.IndexOf(Constants.MetaHeaderSupportedUnits, Unit) < 0) throw new Exception($"Unit {Unit} is not a supported unit: {string.Join(", ", Constants.MetaHeaderSupportedUnits)}");
            if (UpAxis < 0 || UpAxis > 2) throw new Exception("Up axis must be 0(x), 1(y), or 2(z)");
            if (ForwardVector < 0 || ForwardVector > 5) throw new Exception("Front vector must be 0 (x), 1(y), 2(z), 3(-x), 4(-y), or 5(-z)");
            if (Handedness < 0 || Handedness > 1) throw new Exception("Handedness must be 0 (left) or 1 (right");
            return this;
        }

        public static bool IsSegmentMetaHeader(string name, long size)
            => name == Constants.MetaHeaderSegmentName && size == Constants.MetaHeaderSegmentNumBytes;

        public static bool TryRead(Stream stream, long size, out MetaHeader outMetaHeader)
        {
            var buffer = stream.ReadArray<byte>((int)size);

            if (buffer[0] == Constants.MetaHeaderMagicA && buffer[1] == Constants.MetaHeaderMagicB)
            {
                outMetaHeader = FromBytes(buffer);
                return true;
            }
            else
            {
                outMetaHeader = default;
                return false;
            }
        }
    }
}
