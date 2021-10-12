/*
    G3D Geometry Format Library
    Copyright 2019, VIMaec LLC.
    Copyright 2018, Ara 3D Inc.
    Usage licensed under terms of MIT License
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d
{
    /// <summary>
    /// Represents a basic single-precision G3D in memory, with access to common attributes.
    /// The G3D format can be double precision, but this data structure won't provide access to all of the attributes.
    /// In the case of G3D formats that are non-conformant to the expected semantics you can use GeometryAttributes.
    /// This class is inspired heavily by the structure of FBX and Assimp. 
    /// </summary>
    public class G3D : GeometryAttributes
    {
        public new static G3D Empty = Create();

        public G3dHeader Header { get; }

        // These are the values of the most common attributes. Some are retrieved directly from data, others are computed on demand, or coerced. 

        // Vertex buffer. Usually present.
        public IArray<Vector3> Vertices { get; }

        // Index buffer (one index per corner, and per half-edge). Computed if absent. 
        public IArray<int> Indices { get; }

        // Vertex associated data, provided or null
        public List<IArray<Vector2>> AllVertexUvs { get; } = new List<IArray<Vector2>>();
        public List<IArray<Vector4>> AllVertexColors { get; } = new List<IArray<Vector4>>();
        public IArray<Vector2> VertexUvs => AllVertexUvs?.ElementAtOrDefault(0);
        public IArray<Vector4> VertexColors => AllVertexColors?.ElementAtOrDefault(0);
        public IArray<Vector3> VertexNormals { get; }
        public IArray<Vector4> VertexTangents { get; }

        // Face associated data.
        public IArray<int> FaceGroups { get; } // The group that is associated with the face.
        public IArray<int> FaceMaterialIds { get; } // Material id per face, 
        public IArray<Vector3> FaceNormals { get; } // If not provided, are computed dynamically as the average of all vertex normals,

        // A sub-mesh is a section of the vertex and index buffer. Some of these are computed.
        // QUESTION: I might want object ids here in the future.
        public IArray<int> SubGeometryIndexOffsets { get; } // Offset into the index buffer for each SubGeometry
        public IArray<int> SubGeometryVertexOffsets { get; } // Offset into the vertex buffer for each SubGeometry
        public IArray<int> SubGeometryIndexCounts { get; } // Number of indices for each SubGeometry: always computed 
        public IArray<int> SubGeometryVertexCounts { get; } // Number of vertices for each SubGeometry: always computed 
        public IArray<G3dSubGeometry> SubGeometries { get; }

        // Instance data. This is used with sub-meshes. None of these are computed. 
        public IArray<int> InstanceParents { get; } // Index of the parent transform 
        public IArray<Matrix4x4> InstanceTransforms { get; } // A 4x4 matrix in row-column order defining the transormed
        public IArray<int> InstanceSubGeometries { get; } // The SubGeometry associated with the index

        public G3D(IEnumerable<GeometryAttribute> attributes, G3dHeader? header = null, int numCornersPerFaceOverride = -1)
            : base(attributes, numCornersPerFaceOverride)
        {
            Header = header ?? new G3dHeader();

            foreach (var attr in Attributes.ToEnumerable())
            {
                var desc = attr.Descriptor;
                switch (desc.Semantic)
                {
                    case Semantic.Index:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_corner))
                            Indices = Indices ?? attr.AsType<int>().Data;
                        if (attr.IsTypeAndAssociation<short>(Association.assoc_corner))
                            Indices = Indices ?? attr.AsType<short>().Data.Select(x => (int)x);
                        break;

                    case Semantic.Position:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            Vertices = Vertices ?? attr.AsType<Vector3>().Data;
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_corner))
                            Vertices = Vertices ?? attr.AsType<Vector3>().Data;
                        break;

                    case Semantic.Tangent:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            VertexTangents = VertexTangents ?? attr.AsType<Vector3>().Data.Select(v => v.ToVector4());
                        if (attr.IsTypeAndAssociation<Vector4>(Association.assoc_vertex))
                            VertexTangents = VertexTangents ?? attr.AsType<Vector4>().Data;
                        break;

                    case Semantic.Uv:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            AllVertexUvs.Add(attr.AsType<Vector3>().Data.Select(uv => uv.ToVector2()));
                        if (attr.IsTypeAndAssociation<Vector2>(Association.assoc_vertex))
                            AllVertexUvs.Add(attr.AsType<Vector2>().Data);
                        break;

                    case Semantic.Color:
                        if (desc.Association == Association.assoc_vertex)
                            AllVertexColors.Add(attr.AttributeToColors());
                        break;

                    case Semantic.VertexOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_subgeometry))
                            SubGeometryVertexOffsets = SubGeometryVertexOffsets ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.IndexOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_subgeometry))
                            SubGeometryIndexOffsets = SubGeometryIndexOffsets ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Normal:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_face))
                            FaceNormals = FaceNormals ?? attr.AsType<Vector3>().Data;
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            VertexNormals = VertexNormals ?? attr.AsType<Vector3>().Data;
                        break;

                    case Semantic.MaterialId:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_face))
                            FaceMaterialIds = FaceMaterialIds ?? attr.AsType<int>().Data;
                        break;

                    // NOTE: some VIMs have Group and others have GroupId
                    case Semantic.Group:
                    case Semantic.GroupId:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_face))
                            FaceGroups = FaceGroups ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Parent:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_instance))
                            InstanceParents = InstanceParents ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.SubGeometry:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_instance))
                            InstanceSubGeometries = InstanceSubGeometries ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Transform:
                        if (attr.IsTypeAndAssociation<Matrix4x4>(Association.assoc_instance))
                            InstanceTransforms = InstanceTransforms ?? attr.AsType<Matrix4x4>().Data;
                        break;
                }
            }

            // If no vertices are provided, we are going to generate a list of zero vertices.
            if (Vertices == null)
                Vertices = Vector3.Zero.Repeat(0);

            // If no indices are provided then we are going to have to treat the index buffer as indices
            if (Indices == null)
                Indices = Vertices.Indices();

            // Compute face normals if possible
            if (FaceNormals == null && VertexNormals != null)
                FaceNormals = NumFaces.Select(ComputeFaceNormal);

            if (NumSubgeometries > 0)
            {
                if (Indices == null)
                    Debug.WriteLine($"If the number of Subgeometries is greater than zero then the number of index buffer should be present");
                if (Vertices == null)
                    Debug.WriteLine($"If the number of Subgeometries is greater than zero then the vertex buffer is expected");
                if (SubGeometryIndexOffsets == null)
                    Debug.WriteLine($"If the number of Subgeometries is greater than zero then the Subgeometries index buffer is expected ");
                if (SubGeometryVertexOffsets == null)
                    Debug.WriteLine($"If the number of Subgeometries is greater than zero then the Subgeometries vertex buffer is expected ");

                if (SubGeometryIndexCounts == null)
                {
                    if (NumSubgeometries != SubGeometryIndexOffsets.Count)
                        throw new Exception($"Internal error: SubGeometry index offsets count {SubGeometryIndexOffsets.Count} is different than number of Subgeometries {NumSubgeometries}");
                    SubGeometryIndexCounts = NumSubgeometries.Select(i => i < (NumSubgeometries - 1) ? SubGeometryIndexOffsets[i + 1] - SubGeometryIndexOffsets[i] : Indices.Count - SubGeometryIndexOffsets[i]);
                }
                for (var i = 0; i < SubGeometryIndexCounts.Count; ++i)
                {
                    var n = SubGeometryIndexCounts[i];
                    if (n < 0)
                        throw new Exception($"SubGeometry {i} has negative number of indices {n}");

                    if (NumCornersPerFace > 0 && (n % NumCornersPerFace) != 0)
                        throw new Exception($"SubGeometry {i} does not have an index buffer count {n} that is divisible by {NumCornersPerFace}");
                }

                if (SubGeometryVertexCounts == null)
                {
                    if (NumSubgeometries != SubGeometryVertexOffsets.Count)
                        throw new Exception($"Internal error: SubGeometry index offsets count {SubGeometryVertexOffsets.Count} is different than number of Subgeometries {NumSubgeometries}");

                    SubGeometryVertexCounts = NumSubgeometries.Select(i =>
                        i < (NumSubgeometries - 1)
                            ? SubGeometryVertexOffsets[i + 1] - SubGeometryVertexOffsets[i]
                            : Vertices.Count - SubGeometryVertexOffsets[i]
                    );
                }
                for (var i = 0; i < SubGeometryVertexCounts.Count; ++i)
                {
                    var n = SubGeometryVertexCounts[i];
                    if (n < 0)
                        throw new Exception($"SubGeometry {i} has negative number of indices {n}");
                }
            }

            // Compute all of the sub-geometries
            SubGeometries = NumSubgeometries.Select(i => new G3dSubGeometry(this, i));
        }

        public static Vector3 Average(IArray<Vector3> xs)
            => xs.Aggregate(Vector3.Zero, (a, b) => a + b) / xs.Count;

        public Vector3 ComputeFaceNormal(int nFace)
            => Average(NumCornersPerFace.Select(c => VertexNormals[nFace * NumCornersPerFace + c]));

        public static G3D Read(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
                return stream.ReadG3d();
        }

        public static G3D Read(Stream stream)
            => stream.ReadG3d();

        public static G3D Read(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return stream.ReadG3d();
        }

        public static G3D Create(params GeometryAttribute[] attributes)
            => new G3D(attributes);

        public static G3D Create(G3dHeader header, params GeometryAttribute[] attributes)
            => new G3D(attributes, header);
    }
}
