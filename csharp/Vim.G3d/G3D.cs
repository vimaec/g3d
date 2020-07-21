/*
    G3D Geometry Format Library
    Copyright 2019, VIMaec LLC.
    Copyright 2018, Ara 3D Inc.
    Usage licensed under terms of MIT License

    The G3D format is a simple, generic, and efficient representation of geometry data
    that is appropriate for serialization, or for usage in memory. 

    The G3D format was designed to be able to contain a superset of data contained in various common 
    mesh formats including OBJ, FBX, glTF, PLY, and more.  
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vim.G3d
{
    /// <summary>
    /// Represents a single-precision G3D in memory, with access to common attributes.
    /// </summary>
    public class G3D
    {
        public int CornersPerFace { get; }
        public bool IsPolyMesh => CornersPerFace < 0;

        public int NumVertices { get; }
        public int NumFaces { get; }
        public int NumCorners { get; } // Also the number of half-edges 
        public int NumGroups { get; }

        // Commonly accessed attributes (this is a lot of data per mesh)
        public Attribute<float> Vertices; // Arity of 3
        public Attribute<int> Indices; // Arity of 1
        public Attribute<int> FaceSizes; // Number of indices per face, Arity of 1 (could be assigned to a face, group, or object)
        public Attribute<int> GroupIndexOffsets; // Offset into the index buffer for each group, Arity of 1
        public Attribute<int> MaterialIds; // Arity of 1, (per face, per group)
        public Attribute<float> VertexNormal; // Vertex association, Arity of 3
        public Attribute<float> FaceNormal; // Face association, Arity of 3
        public Attribute<float> Tangents; // Arity of 4
        public Attribute<float> Color; // Arity of 4 (face, group)

        // Multiple UV/UVW and VC channels are possible
        public List<Attribute<float>> UV = new List<Attribute<float>>(); // Arity of 2
        public List<Attribute<float>> VertexColor = new List<Attribute<float>>(); // Arity of 3 

        public List<BinaryAttribute> Attributes { get; }

        public Header Header { get; }

        public PolygonGroup[] Groups { get; }

        private int ValidateAttribute(BinaryAttribute attr, int expectedCount)
        {
            if (expectedCount >= 0 && attr.ElementCount != expectedCount)
                throw new Exception($"Attribute {attr.Name} size {attr.ElementCount} not match the expected size {expectedCount}");
            return attr.ElementCount;
        }

        public G3D(IEnumerable<BinaryAttribute> attributes, Header header = null)
        {
            Header = header ?? new Header();

            NumVertices = -1;
            NumFaces = -1;
            NumCorners = -1;
            NumGroups = -1;
            CornersPerFace = -1;

            foreach (var attr in attributes)
            {
                Attributes.Add(attr);

                var desc = attr.Descriptor;

                switch (desc.Association)
                {
                    case Association.assoc_none:
                        break;
                    case Association.assoc_vertex:
                        NumVertices = ValidateAttribute(attr, NumVertices);
                        break;
                    case Association.assoc_edge:
                    case Association.assoc_corner:
                        NumCorners = ValidateAttribute(attr, NumCorners);
                        break;
                    case Association.assoc_face:
                        NumFaces = ValidateAttribute(attr, NumFaces);
                        break;
                    case Association.assoc_group:
                        NumGroups = ValidateAttribute(attr, NumGroups);
                        break;
                }

                switch (desc.Semantic)
                {
                    case Semantic.Position:
                        if (desc.Association == Association.assoc_corner ||
                            desc.Association == Association.assoc_vertex)
                            Vertices = Vertices ?? attr.AsType<float>();
                        break;

                    case Semantic.Index:
                        if (desc.Association == Association.assoc_corner && desc.DataArity == 1)
                            Indices = Indices ?? attr.AsType<int>();

                        // It is not a recommended workflow but it is possible that someone might think it is a good idea to assign
                        // indices to faces with a particular arity. This would suggest that the corners per face value is specified.
                        if (desc.Association == Association.assoc_face)
                            if (CornersPerFace > 0 && CornersPerFace != desc.DataArity)
                                throw new Exception(
                                    $"The number of corners per face is inconsistent expected {CornersPerFace} but found {desc.DataArity}");

                        break;

                    case Semantic.IndexOffset:
                        if (desc.DataArity == 1 && desc.Association == Association.assoc_group)
                            GroupIndexOffsets = GroupIndexOffsets ?? attr.AsType<int>();
                        break;

                    case Semantic.FaceSize:
                        if (desc.Association == Association.assoc_face && desc.DataArity == 1)
                            FaceSizes = FaceSizes ?? attr.AsType<int>();
                        break;

                    case Semantic.Normal:
                        if (desc.Association == Association.assoc_face)
                            FaceNormal = FaceNormal ?? attr.AsType<float>();
                        else if (desc.Association == Association.assoc_vertex)
                            VertexNormal = VertexNormal ?? attr.AsType<float>();
                        break;

                    case Semantic.Uv:
                        if (desc.Association == Association.assoc_vertex && (desc.DataArity == 2 || desc.DataArity == 3))
                            UV.Add(attr.AsType<float>());
                        break;

                    case Semantic.Color:
                        if (desc.Association == Association.assoc_vertex)
                        {
                            if (desc.DataArity == 3 || desc.DataArity == 4)
                                VertexColor.Add(attr.AsType<float>());
                        }
                        else if (desc.DataArity == 4)
                        {
                            Color = Color ?? attr.AsType<float>();
                        }

                        break;

                    case Semantic.Tangent:
                        if (desc.DataArity == 4 && desc.Association == Association.assoc_vertex)
                            Tangents = Tangents ?? attr.AsType<float>();
                        break;

                    case Semantic.MaterialId:
                        if (desc.DataArity == 1)
                            MaterialIds = MaterialIds ?? attr.AsType<int>();
                        break;
                }
            }

            NumVertices = Vertices.ElementCount;

            if (NumVertices < 0)
                throw new Exception("No position data found");

            if (NumCorners < 0)
                NumCorners = NumVertices;

            // Are the face sizes specified? 
            if (FaceSizes != null)
            {
                // Same FaceSize for whole mesh
                if (FaceSizes.Descriptor.Association == Association.assoc_all)
                {
                    if (CornersPerFace < 0)
                        CornersPerFace = FaceSizes.Data[0];
                    else if (CornersPerFace != FaceSizes.Data[0])
                        throw new Exception(
                            $"Number of corner per face was already determined to be {CornersPerFace} but differs from the value of {FaceSizes.Data[0]}");
                }
                else
                {
                    if (CornersPerFace >= 0)
                        throw new Exception($"The number of corners was already determined to be {CornersPerFace} but a face size array was provided");
                }
            }
            else
            {
                // By default we assume triangular meshes
                CornersPerFace = 3;
            }

            if (NumFaces < 0)
            { 
                if (CornersPerFace < 0)
                    throw new Exception("Internal error: expected number of corners to be determined at least from number of positions");

                NumFaces = NumCorners / CornersPerFace;
            }

            if (NumCorners % CornersPerFace != 0)
                throw new Exception($"Internal error: expected number of corners {NumCorners} to be divisble by {CornersPerFace}");
            
            // Compute the polygon groups
            NumGroups = NumGroups >= 0 ? NumGroups: 0;
            Groups = NumGroups > 0
                ? Enumerable.Range(0, NumGroups).Select(i => new PolygonGroup(this, i)).ToArray()
                : new PolygonGroup[0];

            // Further validation: could run through the indices and material ids to assure each one is valid.            
        }

        public static G3D Read(string filePath)
            => BFast.Read(filePath).ToG3D();

        // TODO: I don't think this a great name
        public static G3D Read(byte[] bytes)
            => bytes.Unpack().ToG3D();
    }
}
