using Vim.Math3d;

namespace Vim.G3d.Attributes
{
    [AttributeDescriptor("g3d:all:facesize:0:int32:1", AttributeType.Singleton)]
    public partial class CornersPerFaceAttribute { }

    [AttributeDescriptor("g3d:vertex:position:0:float32:3", AttributeType.Data, ArrayType = typeof(Vector3))]
    public partial class VertexAttribute { }

    [AttributeDescriptor("g3d:corner:index:0:int32:1", AttributeType.Index, IndexInto = typeof(VertexAttribute))]
    public partial class IndexAttribute { }

    [AttributeDescriptor("g3d:instance:transform:0:float32:16", AttributeType.Data, ArrayType = typeof(Matrix4x4))]
    public partial class InstanceTransformAttribute { }

    [AttributeDescriptor("g3d:instance:parent:0:int32:1", AttributeType.Index, IndexInto = typeof(InstanceTransformAttribute))]
    public partial class InstanceParentAttribute { }

    [AttributeDescriptor("g3d:instance:flags:0:uint16:1", AttributeType.Data)]
    public partial class InstanceFlagsAttribute { }

    [AttributeDescriptor("g3d:instance:mesh:0:int32:1", AttributeType.Index, IndexInto = typeof(MeshSubmeshOffsetAttribute))]
    public partial class InstanceMeshAttribute { }

    [AttributeDescriptor("g3d:mesh:submeshoffset:0:int32:1", AttributeType.Index, IndexInto = typeof(SubmeshIndexOffsetAttribute))]
    public partial class MeshSubmeshOffsetAttribute { }

    [AttributeDescriptor("g3d:submesh:indexoffset:0:int32:1", AttributeType.Index, IndexInto = typeof(IndexAttribute))]
    public partial class SubmeshIndexOffsetAttribute { }

    [AttributeDescriptor("g3d:submesh:material:0:int32:1", AttributeType.Index, IndexInto = typeof(MaterialColorAttribute))]
    public partial class SubmeshMaterialAttribute { }

    [AttributeDescriptor("g3d:material:color:0:float32:4", AttributeType.Data, ArrayType = typeof(Vector4))]
    public partial class MaterialColorAttribute { }

    [AttributeDescriptor("g3d:material:glossiness:0:float32:1", AttributeType.Data)]
    public partial class MaterialGlossinessAttribute { }

    [AttributeDescriptor("g3d:material:smoothness:0:float32:1", AttributeType.Data)]
    public partial class MaterialSmoothnessAttribute { }

    [AttributeDescriptor("g3d:shapevertex:position:0:float32:3", AttributeType.Data, ArrayType = typeof(Vector3))]
    public partial class ShapeVertexAttribute { }

    [AttributeDescriptor("g3d:shape:vertexoffset:0:int32:1", AttributeType.Index, IndexInto = typeof(ShapeVertexAttribute))]
    public partial class ShapeVertexOffsetAttribute { }

    [AttributeDescriptor("g3d:shape:color:0:float32:4", AttributeType.Data, ArrayType = typeof(Vector4))]
    public partial class ShapeColorAttribute { }

    [AttributeDescriptor("g3d:shape:width:0:float32:1", AttributeType.Data)]
    public partial class ShapeWidthAttribute { }


    [AttributeCollection(
        typeof(CornersPerFaceAttribute),
        typeof(VertexAttribute),
        typeof(IndexAttribute),
        typeof(InstanceTransformAttribute),
        typeof(InstanceParentAttribute),
        typeof(InstanceFlagsAttribute),
        typeof(InstanceMeshAttribute),
        typeof(MeshSubmeshOffsetAttribute),
        typeof(SubmeshIndexOffsetAttribute),
        typeof(SubmeshMaterialAttribute),
        typeof(MaterialColorAttribute),
        typeof(MaterialGlossinessAttribute),
        typeof(MaterialSmoothnessAttribute),
        typeof(ShapeVertexAttribute),
        typeof(ShapeVertexOffsetAttribute),
        typeof(ShapeColorAttribute),
        typeof(ShapeWidthAttribute))]
    public partial class VimAttributeCollection // : IAttributeCollection
    { }
}
