namespace Vim.G3d
{
    public static class CommonAttributes
    {
        public const string Position = "g3d:vertex:position:0:float32:3";
        public const string Index = "g3d:corner:index:0:int32:1";
        public const string ObjectFaceSize = "g3d:all:facesize:0:int32:1";

        public const string VertexUv = "g3d:vertex:uv:0:float32:2";
        public const string VertexUvw = "g3d:vertex:uv:0:float32:3";
        public const string VertexNormal = "g3d:vertex:normal:0:float32:3";
        public const string VertexColor = "g3d:vertex:color:0:float32:3";
        public const string VertexColorWithAlpha = "g3d:vertex:color:0:float32:4";
        public const string VertexBitangent = "g3d:vertex:bitangent:0:float32:3";
        public const string VertexTangent = "g3d:vertex:tangent:0:float32:3";
        public const string VertexTangent4 = "g3d:vertex:tangent:0:float32:4";
        public const string VertexSelectionWeight = "g3d:vertex:weight:0:float32:1";

        public const string FaceMaterialId = "g3d:face:materialid:0:int32:1";
        public const string FaceObjectId = "g3d:face:objectid:0:int32:1";
        public const string FaceGroupId = "g3d:face:groupid:0:int32:1";
        public const string FaceNormal = "g3d:vertex:normal:0:float32:3";
        public const string FaceSize = "g3d:face:facesize:0:int32:1";
        public const string FaceIndexOffset = "g3d:face:indexoffset:0:int32:1";
        public const string FaceSelectionWeight = "g3d:face:weight:0:float32:1";

        public const string GroupMaterialId = "g3d:group:materialid:0:int32:1";
        public const string GroupObjectId = "g3d:group:objectid:0:int32:1";
        public const string GroupIndexOffset = "g3d:group:indexoffset:0:int32:1";
        public const string GroupNormal = "g3d:vertex:normal:0:float32:3";
        public const string GroupFaceSize = "g3d:group:facesize:0:int32:1";

        // https://docs.thinkboxsoftware.com/products/krakatoa/2.6/1_Documentation/manual/formats/particle_channels.html
        public const string PointVelocity = "g3d:vertex:velocity:0:float32:3";
        public const string PointNormal = "g3d:vertex:normal:0:float32:3";
        public const string PointAcceleration = "g3d:vertex:acceleration:0:float32:3";
        public const string PointDensity = "g3d:vertex:density:0:float32:1";
        public const string PointEmissionColor = "g3d:vertex:emission:0:float32:3";
        public const string PointAbsorptionColor = "g3d:vertex:absorption:0:float32:3";        
        public const string PointSpin = "g3d:vertex:spin:0:float32:4";
        public const string PointOrientation = "g3d:vertex:orientation:0:float32:4";
        public const string PointParticleId = "g3d:vertex:particleid:0:int32:1";
        public const string PointAge = "g3d:vertex:age:0:int32:1";

        // Line specific attributes 
        public const string LineTangentIn = "g3d:vertex:tangent:0:float32:3";
        public const string LineTangentOut = "g3d:vertex:tangent:1:float32:3";
    }
}
