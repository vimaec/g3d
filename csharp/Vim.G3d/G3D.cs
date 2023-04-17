using System;
using System.IO;
using Vim.BFast;
using System.Linq;
using System.Collections.Generic;

namespace Vim.G3d
{
    /// <summary>
    /// A G3d is composed of a header and a collection of attributes containing descriptors and their data.
    /// </summary>
    public class G3d<TAttributeCollection> where TAttributeCollection : IAttributeCollection, new()
    {
        /// <summary>
        /// The meta header of the G3d. Corresponds to the "meta" segment.
        /// </summary>
        public readonly MetaHeader MetaHeader;

        /// <summary>
        /// The attributes of the G3d.
        /// </summary>
        public readonly TAttributeCollection AttributeCollection;

        /// <summary>
        /// Constructor.
        /// </summary>
        public G3d(MetaHeader metaHeader, TAttributeCollection attributeCollection)
        {
            MetaHeader = metaHeader;
            AttributeCollection = attributeCollection;
        }

        /// <summary>
        /// Constructor. Uses the default header.
        /// </summary>
        public G3d(TAttributeCollection attributeCollection) 
            : this(MetaHeader.Default, attributeCollection)
        { }

        /// <summary>
        /// Constructor. Uses the default header and instantiates an attribute collection.
        /// </summary>
        public G3d()
            : this(MetaHeader.Default, new TAttributeCollection())
        { }

        public static G3d<T> Empty<T>() where T : IAttributeCollection, new()
            => new G3d<T>();

        /// <summary>
        /// Reads the stream using the attribute collection's readers and outputs a G3d upon success.
        /// </summary>
        public static bool TryRead(Stream stream, out G3d<TAttributeCollection> g3d)
        {
            g3d = null;
            MetaHeader? metaHeader = null;
            var attributeCollection = new TAttributeCollection();

            object OnG3dSegment(Stream s, string name, long size)
            {
                if (MetaHeader.IsSegmentMetaHeader(name, size) && MetaHeader.TryRead(s, size, out var outMetaHeader))
                {
                    // Assign to the header variable in the closure.
                    return metaHeader = outMetaHeader;
                }

                // The segment is not the header so treat it as an attribute.
                return attributeCollection.ReadAttribute(s, name, size);
            }

            _ = stream.ReadBFast(OnG3dSegment);

            // Failure case if the header was not found.
            if (!metaHeader.HasValue)
                return false;

            try
            {
                // Validate the attribute collection. This may throw an exception.
                attributeCollection.Validate();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Fail(e.Message, e.StackTrace);
                return false;
            }

            // Instantiate the object and return.
            g3d = new G3d<TAttributeCollection>(metaHeader.Value, attributeCollection);
            return true;
        }

        /// <summary>
        /// Reads the g3d from the given byte array. Returns true if the g3d was successfully read.
        /// </summary>
        public static bool TryRead(byte[] bytes, out G3d<TAttributeCollection> g3d)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return TryRead(stream, out g3d);
            }
        }

        /// <summary>
        /// Reads the g3d from the given file path. Returns true if the g3d was successfully read.
        /// </summary>
        public static bool TryRead(string filePath, out G3d<TAttributeCollection> g3d)
        {
            using (var fileStream = new FileInfo(filePath).OpenRead())
            {
                return TryRead(fileStream, out g3d);
            }
        }

        /// <summary>
        /// Returns the G3d BFast header information, including buffer names and buffer sizes in bytes.
        /// </summary>
        private static (BFastHeader BFastHeader, string[] BufferNames, long[] BufferSizesInBytes) GetBFastHeaderInfo(
            NamedBuffer<byte> metaHeaderBuffer,
            IReadOnlyList<IAttribute> attributes)
        {
            var nameList = new List<string>();
            var sizesInBytesList = new List<long>();

            nameList.Add(metaHeaderBuffer.Name);
            sizesInBytesList.Add(metaHeaderBuffer.NumBytes());

            foreach (var attribute in attributes)
            {
                nameList.Add(attribute.Name);
                sizesInBytesList.Add(attribute.GetSizeInBytes());
            }

            var bufferNames = nameList.ToArray();
            var bufferSizesInBytes = sizesInBytesList.ToArray();

            return (
                BFast.BFast.CreateBFastHeader(bufferSizesInBytes, bufferNames),
                bufferNames,
                bufferSizesInBytes
            );
        }

        private delegate void StreamWriter(Stream stream);

        /// <summary>
        /// Writes the G3d to the given stream.
        /// </summary>
        public void Write(Stream stream)
        {
            var metaHeaderBuffer = MetaHeader.ToNamedBuffer();
            var attributes = AttributeCollection.Attributes.Values.OrderBy(n => n.Name).ToArray(); // Order the attributes by name for consistency

            // Prepare the bfast header, which describes the names and ranges.
            var (bfastHeader, bufferNames, bufferSizesInBytes) = GetBFastHeaderInfo(metaHeaderBuffer, attributes);

            // Prepare the stream writers.
            var streamWriters = new StreamWriter[1 + attributes.Length];
            streamWriters[0] = s => s.Write(metaHeaderBuffer); // First stream writer is the "meta" buffer.
            for (int i = 0; i < attributes.Length; i++)
            {
                var attr = attributes[i];
                streamWriters[i + 1] = s => attr.Write(s);
            }

            stream.WriteBFastHeader(bfastHeader);
            stream.WriteBFastBody(bfastHeader, bufferNames, bufferSizesInBytes, (s, index, name, size) =>
            {
                streamWriters[index](s);
                return size;
            });
        }

        /// <summary>
        /// Writes the G3d to the given file path.
        /// </summary>
        public void Write(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fileStream = new FileInfo(filePath).OpenWrite())
                Write(fileStream);
        }

        /// <summary>
        /// Returns a byte array representing the G3d.
        /// </summary>
        public byte[] ToBytes()
        {
            using (var memoryStream = new MemoryStream())
            {
                Write(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Validates the G3d.
        /// </summary>
        public void Validate()
            => AttributeCollection.Validate();
    }
}
