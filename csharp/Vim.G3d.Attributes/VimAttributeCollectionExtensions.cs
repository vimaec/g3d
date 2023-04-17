using System.Linq;

namespace Vim.G3d.Attributes
{
    public static class VimAttributeCollectionExtensions
    {
        public static int GetFaceCount(this VimAttributeCollection c)
        {
            var cornersPerFace = c.GetCornersPerFaceCount();
            var indexCount = c.GetIndexCount();

            return cornersPerFace != 0 ? indexCount / cornersPerFace : -1;
        }

        /// <summary>
        /// The number of corners per face. A value of 3 designates that all faces are triangles (a triangle has 3 corners).
        /// </summary>
        public static int GetCornersPerFaceCount(this VimAttributeCollection c)
            => c.CornersPerFaceAttribute?.TypedData?.FirstOrDefault() ?? 0;

        /// <summary>
        /// The total number of indices.
        /// </summary>
        public static int GetIndexCount(this VimAttributeCollection c)
            => c.IndexAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of vertices.
        /// </summary>
        public static int GetVertexCount(this VimAttributeCollection c)
            => c.VertexAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of instances.
        /// </summary>
        public static int GetInstanceCount(this VimAttributeCollection c)
            => c.InstanceTransformAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of meshes.
        /// </summary>
        public static int GetMeshCount(this VimAttributeCollection c)
            => c.MeshSubmeshOffsetAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of submeshes.
        /// </summary>
        public static int GetSubmeshCount(this VimAttributeCollection c)
            => c.SubmeshIndexOffsetAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of materials.
        /// </summary>
        public static int GetMaterialCount(this VimAttributeCollection c)
            => c.MaterialColorAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of shape vertices.
        /// </summary>
        public static int GetShapeVertexCount(this VimAttributeCollection c)
            => c.ShapeVertexAttribute?.TypedData?.Length ?? 0;

        /// <summary>
        /// The total number of shapes.
        /// </summary>
        public static int GetShapeCount(this VimAttributeCollection c)
            => c.ShapeVertexOffsetAttribute?.TypedData?.Length ?? 0;
    }
}
