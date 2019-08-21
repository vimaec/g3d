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
        public const string FaceSizes = "g3d:face:size:int:1";
        public const string FaceIndices = "g3d:face:indexoffset:int:1";
        public const string VertexColor = "g3d:vertex:color:float32:3";
        public const string VertexColorWithAlpha = "g3d:vertex:color:float32:4";
        public const string Tangent = "g3d:vertex:tangent:float32:3";
        public const string Bitangent = "g3d:vertex:bitangent:float32:3";
        public const string TangentVector4 = "g3d:vertex:tangent:float32:4";
        public const string GroupIndices = "g3d:group:indexoffset:int:1";
        public const string GroupSizes = "g3d:group:size:int:1";
    }
}
