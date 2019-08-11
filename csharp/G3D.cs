/*
    G3D Geometry Format Library
    Copyright 2019, VIMaec LLC.
    Copyright 2018, Ara 3D Inc.
    Usage licensed under terms of MIT License

    The G3D format is a simple, generic, and efficient representation of geometry data. 
    For more information see 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vim.G3D
{
    public class G3D
    {
        public Dictionary<string, Attribute> Attributes { get; }
        public Header Header { get; }

        public G3D(IEnumerable<Attribute> attributes, Header header = null)
        {
            Attributes = attributes.ToDictionary(a => a.Name, a => a);
            Header = header;
        }        
    }

    public static class G3DExtensions
    { 
        public G3D(IEnumerable<IAttribute> attributes, string header = null, bool validate = false)
        {
            Header = header;
            Attributes = attributes;

            foreach (var attr in attributes)
            {
                _AssignIfPossible(attr, ref m_vertexAttribute, AttributeTypeEnum.attr_vertex);
                _AssignIfPossible(attr, ref m_indexAttribute, AttributeTypeEnum.attr_index);
                _AssignIfPossible(attr, ref m_faceSizeAttribute, AttributeTypeEnum.attr_facesize);
                _AssignIfPossible(attr, ref m_faceIndexAttribute, AttributeTypeEnum.attr_faceindex);
                _AssignIfPossible(attr, ref m_materialIdAttribute, AttributeTypeEnum.attr_materialid);
            }

            if (m_vertexAttribute == null)
                throw new Exception("Vertex attribute is not present");

            if (validate)
                this.Validate();
        }

        private static void _AssignIfPossible(IAttribute attr, ref IAttribute target, AttributeTypeEnum at)
        {
            if (attr == null) return;
            if (attr.Descriptor.AttributeType == at)
            {
                if (target != null) throw new Exception("Attribute is already assigned");
                target = attr;
            }
        }

        public static G3D Create(IList<Memory<byte>> buffers)
        {
            if (buffers.Count < 2)
                throw new Exception("Expected at least two data buffers in file: header, and attribute descriptor array");                

            var header = buffers[0].ToBytes().ToUtf8();
            var descriptors = buffers[1].Span.ToStructs<AttributeDescriptor>().ToArray();
            buffers = buffers.Skip(2).ToList();
            if (descriptors.Length != buffers.Count)
                throw new Exception("Incorrect number of descriptors");

            // TODO: this guy is going to be hard to process 
            // I have raw bytes, and I have to cast it to the correct type.
            // That correct type depends on the type flag stored in the descriptor 
            return new G3D(buffers.Zip(descriptors, G3DExtensions.ToAttribute), header);
        }

        // TODO: all of these copies make me die a little bit inside
        public static G3D Create(byte[] bytes)
            => Create(new Memory<byte>(bytes));

        public static G3D Create(Memory<byte> bytes)
            => Create(BFast.ToBFastBuffers(bytes));

        public static G3D ReadFile(string filePath)
            => Create(File.ReadAllBytes(filePath));

        public static G3D Create(IEnumerable<IAttribute> attributes)
            => new G3D(attributes);

        public static G3D Create(params IAttribute[] attributes)
            => new G3D(attributes);
    }
}
