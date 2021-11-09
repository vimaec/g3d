namespace Vim.G3d
{
    /// <summary>
    /// The different types of data types that can be used as elements.
    /// </summary> 
    public enum DataType
    {
        dt_int8,
        dt_int16,
        dt_int32,
        dt_int64,
        dt_float32,
        dt_float64,
        dt_string,
    };

    /// <summary>
    /// What element or component of a mesh each attribute is associated with.  
    /// </summary>  
    public enum Association
    {
        assoc_all,      // associated with all data in G3d
        assoc_none,     // no association
        assoc_vertex,   // vertex or point data 
        assoc_face,     // face associated data
        assoc_corner,   // corner (aka face-vertex) data. A corner is associated with one vertex, but a vertex may be shared between multiple corners  
        assoc_edge,     // half-edge data. Each face consists of n half-edges, one per corner. A half-edge, is a directed edge
        assoc_subgeometry,  // a contiguous section of the main geometry. This is used for instancing. It may be associated with an object.
        assoc_instance, // instance information 
    };

    /// <summary>
    /// Common semantic names.
    /// </summary>
    public static class Semantic
    {
        public const string Position = "position";
        public const string Index = "index";
        public const string FaceSize = "facesize";
        public const string Uv = "uv";
        public const string Normal = "normal";
        public const string Color = "color";
        public const string Bitangent = "bitangent";
        public const string Tangent = "tangent";
        public const string Weight = "weight";

        // Usually associated with face.
        public const string MaterialId = "materialid";

        // Usually associated with sub-geometries 
        public const string IndexOffset = "indexoffset";
        public const string VertexOffset = "vertexoffset";

        // Usually associated with instances
        public const string SubGeometry = "subgeometry";
        public const string Parent = "parent";
        public const string Transform = "transform";

        // Legacy
        public const string Group = "group";
        public const string GroupId = "groupid";

        public const string TangentInt = "tangentin";
        public const string TangentOut = "tangentout";

    }
}
