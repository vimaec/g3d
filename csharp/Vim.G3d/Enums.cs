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
    };

    /// <summary>
    /// What element of a mesh each attribute is associated with.  
    /// </summary>  
    public enum Association
    {
        assoc_all,      // entire geometry
        assoc_none,     // no association
        assoc_vertex,   // vertex data 
        assoc_face,     // face data
        assoc_corner,   // corner data (each face has n corners)
        assoc_edge,     // half-edge data (each face consists of n half-edges, one per corner)
        assoc_group,    // face-group (each face-group is a contiguous series of faces, and can be thought of as a surface or sub-mesh)
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

        public const string MaterialId = "materialid";
        public const string ObjectId = "objectid";
        public const string GroupId = "groupid";
        public const string IndexOffset = "indexoffset";
        
        public const string Velocity = "velocity";
        public const string Acceleration = "acceleration";
        public const string Density = "density";
        public const string Emission = "emission";
        public const string Absorption = "absorption";
        public const string Spin = "spin";
        public const string Orientaton = "orientation";
        public const string ParticleId = "particleid";
        public const string Age = "age";

        // Line specific attributes 
        public const string TangentInt = "tangentin";
        public const string TangentOut = "tangentout";

    }
}
