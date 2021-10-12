using System.Collections.Generic;
using Vim.LinqArray;
using Vim.Math3d;

namespace Vim.G3d
{
    /// <summary>
    /// A subgeometry is a section of the G3D data.
    /// This does not implement IGeometryAttributes for performance reasons. 
    /// </summary>
    public class G3dSubGeometry
    {
        public G3D G3D { get; }
        public int Index { get; }

        public int VertexOffset => G3D.SubGeometryVertexOffsets[Index];
        public int NumVertices => G3D.SubGeometryVertexCounts[Index];
        public int IndexOffset => G3D.SubGeometryIndexOffsets[Index];
        public int NumCorners => G3D.SubGeometryIndexCounts[Index];
        public int FaceOffset => IndexOffset / NumCornersPerFace;
        public int NumFaces => NumCorners / NumCornersPerFace;
        public int NumCornersPerFace => G3D.NumCornersPerFace;

        public G3dSubGeometry(G3D parent, int index)
        {
            (G3D, Index) = (parent, index);
            Vertices = G3D.Vertices?.SubArray(VertexOffset, NumVertices);
            var offset = VertexOffset;
            Indices = G3D.Indices?.SubArray(IndexOffset, NumCorners).Select(i => i - offset);
            VertexUvs = G3D.VertexUvs?.SubArray(VertexOffset, NumVertices);
            VertexNormals = G3D.VertexNormals?.SubArray(VertexOffset, NumVertices);
            VertexColors = G3D.VertexColors?.SubArray(VertexOffset, NumVertices);
            VertexTangents = G3D.VertexTangents?.SubArray(VertexOffset, NumVertices);
            FaceGroups = G3D.FaceGroups?.SubArray(FaceOffset, NumFaces);
            FaceNormals = G3D.FaceNormals?.SubArray(FaceOffset, NumFaces);
            FaceMaterialIds = G3D.FaceMaterialIds?.SubArray(FaceOffset, NumFaces);
        }

        // Vertex buffer. Usually present.
        public IArray<Vector3> Vertices { get; }

        // Index buffer (one index per corner, and per half-edge)
        public IArray<int> Indices { get; }

        // Vertex associated data
        public IArray<Vector2> VertexUvs { get; }
        public IArray<Vector3> VertexNormals { get; }
        public IArray<Vector4> VertexColors { get; }
        public IArray<Vector4> VertexTangents { get; }

        // Face associated data.
        public IArray<int> FaceGroups { get; }
        public IArray<Vector3> FaceNormals { get; }
        public IArray<Vector4> FaceColors { get; }
        public IArray<int> FaceMaterialIds { get; }
        public IArray<int> FaceObjectIds { get; }
    }
}
