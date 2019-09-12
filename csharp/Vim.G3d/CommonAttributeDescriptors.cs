namespace Vim.G3d
{
    public static class CommonAttributes
    {
        public const string Position = "g3d:vertex:position:float32:3";
        public const string Indices = "g3d:corner:index:int32:1";
        public const string UV = "g3d:vertex:uv:float32:2";
        public const string UVW = "g3d:vertex:uv:float32:3";
        public const string VertexNormal = "g3d:vertex:normal:float32:3";
        public const string FaceNormal = "g3d:vertex:normal:float32:3";
        public const string ObjectFaceSize = "g3d:all:size:int32:1";
        public const string GroupFaceSize = "g3d:group:size:int32:1";
        public const string FaceSizes = "g3d:face:size:int32:1";
        public const string FaceIndices = "g3d:face:indexoffset:int32:1";
        public const string VertexColor = "g3d:vertex:color:float32:3";
        public const string VertexColorWithAlpha = "g3d:vertex:color:float32:4";
        public const string Bitangent = "g3d:vertex:bitangent:float32:3";
        public const string Tangent3 = "g3d:vertex:tangent:float32:3";
        public const string Tangent4 = "g3d:vertex:tangent:float32:4";
        public const string GroupIndices = "g3d:group:indexoffset:int32:1";
        public const string MaterialIds = "g3d:face:materialid:int32:1";
    }
}
