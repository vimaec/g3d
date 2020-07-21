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
        public static G3D Empty = Create();

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
        public IArray<int> FaceGroups { get; } // The group that is associated with the face. Used for computing a lot of the face data 
        public IArray<Vector3> FaceNormals { get; } // If not provided, are computed dynamically as the average of all vertex normals,
        public IArray<Vector4> FaceColors { get; } // If not provided, computed from group
        public IArray<int> FaceMaterialIds { get; } // Material id per face, but might also be found in the group (more efficient) 
        public IArray<int> FaceObjectIds { get; } // The object id associated with the face, but might also be found in the group (more efficient)

        // Data associated with groups of faces. None are computed, except for possibly colors. 
        public IArray<Vector4> GroupColors { get; } // Colors. Retrieved from material if not provided 
        public IArray<int> GroupObjectIds { get; } // An ID for the Object associated with this group, multiple groups might share an object Id
        public IArray<int> GroupIds { get; } // A numerical value associated with a group, that might not be its index. 
        public IArray<int> GroupMaterialIds { get; } // Material id associated with group.

        // A sub-mesh is a section of the vertex and index buffer. Some of these are computed.
        // QUESTION: I might want object ids here in the future.
        public IArray<int> SubGeometryIndexOffsets { get; } // Offset into the index buffer for each SubGeometry
        public IArray<int> SubGeometryVertexOffsets { get; } // Offset into the vertex buffer for each SubGeometry
        public IArray<int> SubGeometryIndexCounts { get; } // Number of indices for each SubGeometry: always computed 
        public IArray<int> SubGeometryVertexCounts { get; } // Number of vertices for each SubGeometry: always computed 
        public IArray<G3dSubGeometry> SubGeometries { get; }

        // Instance data. This is used with sub-meshes. None of these are computed. 
        public IArray<int> InstanceIds { get; } // A numerical value representing the instance, that might not be the index.
        public IArray<int> InstanceParents { get; } // Index of the parent transform 
        public IArray<Matrix4x4> InstanceTransforms { get; } // A 4x4 matrix in row-column order defining the transormed
        public IArray<int> InstanceSubGeometries { get; } // The SubGeometry associated with the index
        
        public G3D(IEnumerable<GeometryAttribute> attributes, G3dHeader? header = null)
            : base(attributes)
        {
            Header = header ?? new G3dHeader();
            
            foreach (var attr in Attributes.ToEnumerable())
            {
                var desc = attr.Descriptor;
                switch (desc.Semantic)
                {
                    case Semantic.Position:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            Vertices = Vertices ?? attr.AsType<Vector3>().Data;
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_corner))
                            Vertices = Vertices ?? attr.AsType<Vector3>().Data;
                        break;

                    case Semantic.Index:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_corner))
                            Indices = Indices ?? attr.AsType<int>().Data;
                        if (attr.IsTypeAndAssociation<short>(Association.assoc_corner))
                            Indices = Indices ?? attr.AsType<short>().Data.Select(x => (int)x);
                        break;

                    case Semantic.Id:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_instance))
                            InstanceIds = InstanceIds ?? attr.AsType<int>().Data;
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_group))
                            GroupIds = GroupIds ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.VertexOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_subgeo))
                            SubGeometryVertexOffsets = SubGeometryVertexOffsets ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.IndexOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_subgeo))
                            SubGeometryIndexOffsets = SubGeometryIndexOffsets ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Normal:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_face))
                            FaceNormals = FaceNormals ?? attr.AsType<Vector3>().Data;
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            VertexNormals = VertexNormals ?? attr.AsType<Vector3>().Data;
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
                        if (desc.Association == Association.assoc_face)
                            FaceColors = FaceColors?? attr.AttributeToColors();
                        if (desc.Association == Association.assoc_group)
                            GroupColors = GroupColors ?? attr.AttributeToColors();
                        break;

                    case Semantic.MaterialId:
                        if (desc.DataArity == 1 && desc.Association == Association.assoc_face)
                            FaceMaterialIds = FaceMaterialIds ?? attr.AsType<int>().Data;
                        if (desc.DataArity == 1 && desc.Association == Association.assoc_group)
                            GroupMaterialIds = GroupMaterialIds ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Group:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_face))
                            FaceGroups = FaceGroups ?? attr.AsType<int>().Data;
                        break;
                }
            }

            // If no vertices are provided, we are going to generate a list of zero vertices.
            if (Vertices == null)
                Vertices = Vector3.Zero.Repeat(0);

            // If no indices are provided then we are going to have to treat the index buffer as indices
            if (Indices == null)
                Indices = Vertices.Indices();

            // If we don't have certain data associated with faces, but we do with groups, we can automatically look it up by indexing into the groups
            if (FaceMaterialIds == null && GroupMaterialIds != null && FaceGroups != null)
                FaceMaterialIds = FaceGroups.Select(g => g >= 0 ? GroupMaterialIds[g] : -1);
            var defaultColor = new Vector4(0.5f, 0.5f, 0.5f, 1);
            if (FaceColors == null && GroupColors != null && FaceGroups != null)
                FaceColors = FaceGroups.Select(g => g >= 0 ? GroupColors[g] : defaultColor);
            if (FaceObjectIds == null && GroupObjectIds != null && FaceGroups != null)
                FaceObjectIds = FaceGroups.Select(g => g >= 0 ? FaceObjectIds[g] : -1);

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
                    SubGeometryIndexCounts = NumGroups.Select(i => i < NumGroups ? SubGeometryIndexOffsets[i + 1] - SubGeometryIndexOffsets[i] : Indices.Count - SubGeometryIndexOffsets[i]);
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

                    SubGeometryVertexCounts = NumGroups.Select(i => i < NumGroups ? SubGeometryVertexOffsets[i + 1] - SubGeometryVertexOffsets[i] : Vertices.Count - SubGeometryVertexOffsets[i]);
                }
                for (var i = 0; i < SubGeometryVertexCounts.Count; ++i)
                {
                    var n = SubGeometryVertexCounts[i];
                    if (n < 0)
                        throw new Exception($"SubGeometry {i} has negative number of indices {n}");

                    if (NumCornersPerFace > 0 && (n % NumCornersPerFace) != 0)
                        throw new Exception($"SubGeometry {i} does not have an vertex buffer count {n} that is divisible by {NumCornersPerFace}");
                }
            }

            // Compute all of the sub-geometries
            SubGeometries = NumSubgeometries.Select(i => new G3dSubGeometry(this, i)).Evaluate();
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
