/*
    G3D Geometry Format Library
    Copyright 2019, VIMaec LLC.
    Copyright 2018, Ara 3D Inc.
    Usage licensed under terms of MIT License

    The G3D format is a simple, generic, and efficient representation of geometry data. 
*/

using System;
using System.Collections.Generic;

namespace Vim.G3d
{
    public class G3D
    {
        public int CornersPerFace { get; }
        public bool IsPolyMesh => CornersPerFace < 0;

        public int NumVertices { get; }
        public int NumFaces { get; }
        public int NumCorners { get; } // Also the number of half-edges 
        public int NumGroups { get; }
        public int NumObjects { get; }

        // Commonly accessed attributes 
        public Attribute<float> Vertices;
        public Attribute<int> Indices;
        public Attribute<int> FaceIndices;
        public Attribute<int> FaceSizes;
        public Attribute<int> GroupIndices;
        public Attribute<int> GroupSizes;
        public Attribute<float> VertexNormal;
        public Attribute<float> FaceNormal;
        public Attribute<float> UV;
        public Attribute<float> UVW;

        public List<Attribute> Attributes = new List<Attribute>();

        public Header Header { get; }

        private int AssignAttribute(Attribute attr, int expectedCount)
        {
            if (expectedCount >= 0 && attr.Count != expectedCount)
                throw new Exception($"Attribute {attr.Name} size {attr.Count} not match the expected size {expectedCount}");
            return attr.Count;
        }

        public G3D(IEnumerable<Attribute> attributes, Header header = null)
        {
            Header = header;

            NumFaces = -1;
            NumCorners = -1;
            NumGroups = -1;
            NumObjects = -1;

            foreach (var attr in attributes)
            {
                Attributes.Add(attr);

                switch (attr.Descriptor.Association)
                {
                    case AssociationEnum.assoc_none:
                        break;
                    case AssociationEnum.assoc_edge:
                    case AssociationEnum.assoc_corner:
                        NumCorners = AssignAttribute(attr, NumCorners);
                        break;
                    case AssociationEnum.assoc_face:
                        NumFaces = AssignAttribute(attr, NumFaces);
                        break;
                    case AssociationEnum.assoc_group:
                        NumGroups = AssignAttribute(attr, NumGroups);
                        break;
                    case AssociationEnum.assoc_object:
                        NumObjects = AssignAttribute(attr, NumObjects);
                        break;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_position && attr.Descriptor.Association == AssociationEnum.assoc_corner)
                {
                    Vertices = Vertices ?? attr;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_index && attr.Descriptor.Association == AssociationEnum.assoc_corner)
                {
                    Indices = Indices ?? attr;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_size && 
                    (attr.Descriptor.Association == AssociationEnum.assoc_face || attr.Descriptor.Association == AssociationEnum.assoc_object))
                {
                    FaceSizes = FaceSizes ?? attr;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_normal && attr.Descriptor.Association == AssociationEnum.assoc_face)
                {
                    FaceNormal = FaceNormal ?? attr;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_normal && attr.Descriptor.Association == AssociationEnum.assoc_vertex)
                {
                    VertexNormal = VertexNormal ?? attr;
                }

                if (attr.Descriptor.Semantic == SemanticEnum.sem_uv && attr.Descriptor.Association == AssociationEnum.assoc_vertex)
                {
                    UV = UV ?? attr;
                }
            }

            if (NumVertices < 0)
                throw new Exception("No position data found");

            if (NumCorners < 0)
                NumCorners = NumVertices;

            if (NumObjects < 0)
                NumObjects = 1;

            CornersPerFace = -1;

            // Are the face sizes specified? 
            if (FaceSizes != null)
            {
                // Same FaceSize for whole mesh
                if (FaceSizes.Descriptor.Association == AssociationEnum.assoc_object)
                {
                    CornersPerFace = FaceSizes.CastData<int>()[0];
                }
                else
                {
                    CornersPerFace = -1;
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

                if (NumCorners % CornersPerFace != 0)
                    throw new Exception($"Couldn't determine number of faces, and expected number of corners to be divisble by {CornersPerFace}");

                NumFaces = NumCorners / CornersPerFace;
            }
        }        
    }
}
