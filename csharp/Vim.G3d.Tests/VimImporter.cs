using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Vim.BFast;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d
{
    public class SimpleVimDocument
    {
        public G3D Geometry;
        public string Header;
        public IArray<Matrix4x4> Transforms => Geometry.InstanceTransforms;
    }

    public class LegacyVimDocument
    {
        public G3D Geometry;
        public string Header;
        public IArray<SerializableSceneNode> Nodes { get; set; }
        public IArray<Matrix4x4> Transforms => Nodes.Select(n => n.Transform);
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SerializableSceneNode
    {
        public int Parent;
        public int Geometry;
        public int Instance;
        public Matrix4x4 Transform;
    }

    public static class VimImporter
    {
        /*
        public static G3D ToV1(this G3D g3d, IArray<Matrix4x4> matrices, IArray<int> instances)
        {
            var tmp = g3d.Set
        }

        public static SimpleVimDocument ToV1(this LegacyVimDocument doc)
            => new SimpleVimDocument
            {
                Geometry = doc.Geometry.ToV1(doc.Transforms),
                Header = doc.Header,
            };

        public static SimpleVimDocument ReadLegacyVim(Stream stream)
            => ReadLegacy(stream).ToV1();
        */

        public static LegacyVimDocument ReadLegacyVim(Stream stream)
        {
            var r = new LegacyVimDocument();

            // Reads the geometry and the transform graph from older VIM files.
            stream.ReadBFast((stm, name, size) =>
            {
                if (name == "geometry")
                {
                    stm.ReadG3d();
                }
                else if (name == "nodes")
                {
                    var nodeSize = 19 * 4;
                    var nodeCount = size / nodeSize;
                    if (size % nodeSize != 0)
                        throw new Exception($"Number of bytes {size} does not divide by sizeof nodes {nodeSize}");
                    r.Nodes = stm.ReadArray<SerializableSceneNode>((int)nodeCount).ToIArray();
                }
                else if (name == "header")
                {
                    var bytes = stm.ReadArray<byte>((int)size);
                    r.Header = Encoding.UTF8.GetString(bytes);
                }
                return r;
            });

            return r;
        }
    }
}
