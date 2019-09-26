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
    /// The semantic (role or meaning) of the attribute. These are commonly defined roles in geometry processing applications.    
    /// </summary>
    public enum Semantic
    {
        sem_unknown,       // no known attribute type
        sem_position,      // vertex buffer 
        sem_index,         // index buffer
        sem_indexoffset,   // an offset into the index buffer (used with groups and with faces)
        sem_normal,        // computed normal information (per face, group, corner, or vertex)
        sem_binormal,      // computed binormal information (per vertex_
        sem_tangent,       // computed tangent information (per vertex)
        sem_materialid,    // material id (per face, per group)
        sem_visibility,    // visibility data (e.g. 
        sem_facesize,      // number of indices per face: can be per face, per group, or per object. 
        sem_uv,            // UV or UVW (sometimes more than 1, e.g. Unity supports up to 8)
        sem_color,         // usually vertex color, but could be edge color as well
        sem_smoothing,     // identifies smoothing groups (e.g. ala 3ds Max and OBJ files)
        sem_weight,        // in 3ds Max this is called selection 
        sem_mapchannel,    // 3ds Max map channel (assoc of none => map verts, assoc of corner => map faces)
        sem_id,            // used to identify what object each face part came from 
        sem_joint,         // used to identify what a joint a skin is associated with 
        sem_boxes,         // used to identify bounding boxes
        sem_spheres,       // used to identify bounding spheres
        sem_geometryid,    // identifies geometry per instance
        sem_user,          // identifies user specific data (in 3ds Max this could be "per-vertex-data")
    };
}
