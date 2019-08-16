using System.Collections.Generic;

namespace Vim.G3D
{
    /// <summary>
    /// Maps a name to a pattern for matching attribute descriptors 
    /// </summary>
    public class Schema : Dictionary<string, string>
    { }

    /// <summary>
    /// Contains the schemas of several popular mesh formats    
    /// </summary>
    public class Schemas
    {
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/Mesh.html
        /// </summary>
        public Schema Unity { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "index", "g3d:corner:index:0:(int16|int32):1" },
            { "uv", "g3d:vertex:uv:[0..7]:float32:2" },
            { "normal", "g3d:vertex:normal:0:float32:3" },
            { "tangent", "g3d:vertex:tangent:0:float32:4" },
            { "materialids", "g3d:group:materialid:0:int32:1" },
            { "submeshindices", "g3d:group:indexoffset:0:int32:1" },
            { "submeshsizes", "g3d:group:size:0:int32:1" },
            { "submeshvertexoffsets", "g3d:group:vertexoffset:0:int32:1" },
        };

        /// <summary>
        /// A simple schema for triangular meshes 
        /// </summary>
        public Schema TriMesh { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "index", "g3d:corner:index:0:int32:1" },
        };

        /// <summary>
        /// http://paulbourke.net/dataformats/stl/
        /// </summary>
        public Schema STL { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "normal", "g3d:face:index:0:int32:1" },
        };

        /// <summary>
        /// http://paulbourke.net/dataformats/obj/
        /// </summary>
        public Schema ObjPolyMesh { get; } = new Schema()
        {
            { "vertexdata", "g3d:vertex:position:0:float32:3" },
            { "normaldata", "g3d:none:normal:0:float32:3" },
            { "texturedata", "g3d:none:normal:0:float32:3" },
            { "vertexindex", "g3d:corner:index:0:int32:1" },
            { "normalindex", "g3d:corner:index:0:int32:1" },
            { "textureindex", "g3d:corner:index:0:int32:1" },
            { "facesize", "g3d:face:size:0:int32:1" },
            { "faceoffset", "g3d:face:indexoffset:0:int32:1" },
            { "smoothing", "g3d:face:smoothing:0:int32:1" },
            { "groupname", "g3d:group:id:0:string:1" },
        };

        /// <summary>
        /// https://github.com/mrdoob/three.js/wiki/JSON-Geometry-format-4
        /// https://github.com/mrdoob/three.js/blob/master/src/core/BufferGeometry.js
        /// </summary>
        public Schema ThreeJs { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "normal", "g3d:vertex:normal:0:float32:3" },
            { "uv", "g3d:vertex:uv:0:float32:2" },
            { "tangent", "g3d:vertex:uv:0:float32:4" },
            { "color", "g3d:vertex:color:0:float32:3" },
        };

        /// <summary>
        /// http://assimp.sourceforge.net/lib_html/structai_mesh.html
        /// </summary>
        public Schema Assimmp { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "uv", "g3d:vertex:uv:\\d:float32:3" },
            { "tangent", "g3d:vertex:uv:0:float32:3" },
            { "bitangent", "g3d:vertex:uv:0:float32:3" },
            { "color", "g3d:vertex:color:*:float32:4" },
            { "normal", "g3d:vertex:color:0:float32:3" },
        };

        // https://help.autodesk.com/view/3DSMAX/2017/ENU/?guid=__cpp_ref_class_mesh_html
        public Schema StudioMax { get; } = new Schema()
        {
            { "position", "g3d:vertex:point:0:float32:3" },
            { "index", "g3d:corner:index:0:int32:1" },
            { "vertexdata", "g3d:corner:user:\\d+:float32:1" },
            { "mapdata", "g3d:none:mapchannel:\\d+:float32:3" },
            { "mapindex", "g3d:corner:mapchannel:\\d+:int32:1" },
            { "edgevis", "g3d:edge:visibility:0:int32:1" },
            { "smgroup", "g3d:face:smoothing:0:int32:1" },
            { "materialid", "g3d:face:materialid:0:int32:1" },
        };

        // https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/README.md#meshes
        public Schema Gltf { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float32:3" },
            { "normal", "g3d:vertex:normal:0:float32:3" },
            { "texcoord0", "g3d:vertex:uv:0:(float32|int8|int16):2" },
            { "texcoord1", "g3d:vertex:uv:1:(float32|int8|int16):2" },
            { "tangent", "g3d:vertex:uv:0:float32:4" },
            { "color0", "g3d:vertex:color:0:(float32|int8|int16):(3|4)" },
            { "joints", "g3d:vertex:joint:0:(int8|int16):4" },
            { "weights", "g3d:vertex:weighs:0:(float|int8|int16):4" },
        };

        // http://help.autodesk.com/view/FBX/2017/ENU/?guid=__files_GUID_5EDC0280_E000_4B0B_88DF_5D215A589D5E_htm
        public Schema Fbx { get; } = new Schema()
        {
            { "position", "g3d:vertex:position:0:float64:4" },
        };
    }
}

