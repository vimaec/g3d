// AUTO-GENERATED FILE, DO NOT MODIFY.
// ReSharper disable All
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Vim.BFast;

namespace Vim.G3d.Attributes
{
    
    public partial class CornersPerFaceAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:all:facesize:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<CornersPerFaceAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Singleton;

        public Type IndexInto { get; }
            = null;

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class VertexAttribute : IAttribute<Vim.Math3d.Vector3>
    {
        public const string AttributeName = "g3d:vertex:position:0:float32:3";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<VertexAttribute, Vim.Math3d.Vector3>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public Vim.Math3d.Vector3[] TypedData { get; set; }
            = Array.Empty<Vim.Math3d.Vector3>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class IndexAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:corner:index:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<IndexAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.VertexAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class InstanceTransformAttribute : IAttribute<Vim.Math3d.Matrix4x4>
    {
        public const string AttributeName = "g3d:instance:transform:0:float32:16";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<InstanceTransformAttribute, Vim.Math3d.Matrix4x4>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public Vim.Math3d.Matrix4x4[] TypedData { get; set; }
            = Array.Empty<Vim.Math3d.Matrix4x4>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class InstanceParentAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:instance:parent:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<InstanceParentAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.InstanceTransformAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class InstanceFlagsAttribute : IAttribute<System.UInt16>
    {
        public const string AttributeName = "g3d:instance:flags:0:uint16:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<InstanceFlagsAttribute, System.UInt16>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public System.UInt16[] TypedData { get; set; }
            = Array.Empty<System.UInt16>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class InstanceMeshAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:instance:mesh:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<InstanceMeshAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.MeshSubmeshOffsetAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class MeshSubmeshOffsetAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:mesh:submeshoffset:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<MeshSubmeshOffsetAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.SubmeshIndexOffsetAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class SubmeshIndexOffsetAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:submesh:indexoffset:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<SubmeshIndexOffsetAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.IndexAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class SubmeshMaterialAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:submesh:material:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<SubmeshMaterialAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.MaterialColorAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class MaterialColorAttribute : IAttribute<Vim.Math3d.Vector4>
    {
        public const string AttributeName = "g3d:material:color:0:float32:4";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<MaterialColorAttribute, Vim.Math3d.Vector4>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public Vim.Math3d.Vector4[] TypedData { get; set; }
            = Array.Empty<Vim.Math3d.Vector4>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class MaterialGlossinessAttribute : IAttribute<System.Single>
    {
        public const string AttributeName = "g3d:material:glossiness:0:float32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<MaterialGlossinessAttribute, System.Single>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public System.Single[] TypedData { get; set; }
            = Array.Empty<System.Single>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class MaterialSmoothnessAttribute : IAttribute<System.Single>
    {
        public const string AttributeName = "g3d:material:smoothness:0:float32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<MaterialSmoothnessAttribute, System.Single>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public System.Single[] TypedData { get; set; }
            = Array.Empty<System.Single>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class ShapeVertexAttribute : IAttribute<Vim.Math3d.Vector3>
    {
        public const string AttributeName = "g3d:shapevertex:position:0:float32:3";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<ShapeVertexAttribute, Vim.Math3d.Vector3>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public Vim.Math3d.Vector3[] TypedData { get; set; }
            = Array.Empty<Vim.Math3d.Vector3>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class ShapeVertexOffsetAttribute : IAttribute<System.Int32>
    {
        public const string AttributeName = "g3d:shape:vertexoffset:0:int32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<ShapeVertexOffsetAttribute, System.Int32>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Index;

        public Type IndexInto { get; }
            = typeof(Vim.G3d.Attributes.ShapeVertexAttribute);

        public System.Int32[] TypedData { get; set; }
            = Array.Empty<System.Int32>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class ShapeColorAttribute : IAttribute<Vim.Math3d.Vector4>
    {
        public const string AttributeName = "g3d:shape:color:0:float32:4";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<ShapeColorAttribute, Vim.Math3d.Vector4>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public Vim.Math3d.Vector4[] TypedData { get; set; }
            = Array.Empty<Vim.Math3d.Vector4>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
    
    public partial class ShapeWidthAttribute : IAttribute<System.Single>
    {
        public const string AttributeName = "g3d:shape:width:0:float32:1";

        public string Name
            => AttributeName;

        public static AttributeReader CreateAttributeReader()
            => AttributeCollectionExtensions.CreateAttributeReader<ShapeWidthAttribute, System.Single>();

        public IAttributeDescriptor AttributeDescriptor { get; }
            = new AttributeDescriptor(AttributeName);

        public AttributeType AttributeType { get; }
            = AttributeType.Data;

        public Type IndexInto { get; }
            = null;

        public System.Single[] TypedData { get; set; }
            = Array.Empty<System.Single>();

        public Array Data
            => TypedData;

        public void Write(Stream stream)
        {
            if (TypedData == null || TypedData.Length == 0)
                return;
            stream.Write(TypedData);
        }
    }
        public partial class VimAttributeCollection : IAttributeCollection
    {
        public IEnumerable<string> AttributeNames
            => Attributes.Keys;

        public IDictionary<string, IAttribute> Attributes { get; }
            = new Dictionary<string, IAttribute>
            {
                [Vim.G3d.Attributes.CornersPerFaceAttribute.AttributeName] = new Vim.G3d.Attributes.CornersPerFaceAttribute(),
                [Vim.G3d.Attributes.VertexAttribute.AttributeName] = new Vim.G3d.Attributes.VertexAttribute(),
                [Vim.G3d.Attributes.IndexAttribute.AttributeName] = new Vim.G3d.Attributes.IndexAttribute(),
                [Vim.G3d.Attributes.InstanceTransformAttribute.AttributeName] = new Vim.G3d.Attributes.InstanceTransformAttribute(),
                [Vim.G3d.Attributes.InstanceParentAttribute.AttributeName] = new Vim.G3d.Attributes.InstanceParentAttribute(),
                [Vim.G3d.Attributes.InstanceFlagsAttribute.AttributeName] = new Vim.G3d.Attributes.InstanceFlagsAttribute(),
                [Vim.G3d.Attributes.InstanceMeshAttribute.AttributeName] = new Vim.G3d.Attributes.InstanceMeshAttribute(),
                [Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.AttributeName] = new Vim.G3d.Attributes.MeshSubmeshOffsetAttribute(),
                [Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.AttributeName] = new Vim.G3d.Attributes.SubmeshIndexOffsetAttribute(),
                [Vim.G3d.Attributes.SubmeshMaterialAttribute.AttributeName] = new Vim.G3d.Attributes.SubmeshMaterialAttribute(),
                [Vim.G3d.Attributes.MaterialColorAttribute.AttributeName] = new Vim.G3d.Attributes.MaterialColorAttribute(),
                [Vim.G3d.Attributes.MaterialGlossinessAttribute.AttributeName] = new Vim.G3d.Attributes.MaterialGlossinessAttribute(),
                [Vim.G3d.Attributes.MaterialSmoothnessAttribute.AttributeName] = new Vim.G3d.Attributes.MaterialSmoothnessAttribute(),
                [Vim.G3d.Attributes.ShapeVertexAttribute.AttributeName] = new Vim.G3d.Attributes.ShapeVertexAttribute(),
                [Vim.G3d.Attributes.ShapeVertexOffsetAttribute.AttributeName] = new Vim.G3d.Attributes.ShapeVertexOffsetAttribute(),
                [Vim.G3d.Attributes.ShapeColorAttribute.AttributeName] = new Vim.G3d.Attributes.ShapeColorAttribute(),
                [Vim.G3d.Attributes.ShapeWidthAttribute.AttributeName] = new Vim.G3d.Attributes.ShapeWidthAttribute(),
            };

        public IDictionary<string, AttributeReader> AttributeReaders { get; }
            = new Dictionary<string, AttributeReader>
            {
                [Vim.G3d.Attributes.CornersPerFaceAttribute.AttributeName] = Vim.G3d.Attributes.CornersPerFaceAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.VertexAttribute.AttributeName] = Vim.G3d.Attributes.VertexAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.IndexAttribute.AttributeName] = Vim.G3d.Attributes.IndexAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.InstanceTransformAttribute.AttributeName] = Vim.G3d.Attributes.InstanceTransformAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.InstanceParentAttribute.AttributeName] = Vim.G3d.Attributes.InstanceParentAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.InstanceFlagsAttribute.AttributeName] = Vim.G3d.Attributes.InstanceFlagsAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.InstanceMeshAttribute.AttributeName] = Vim.G3d.Attributes.InstanceMeshAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.AttributeName] = Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.AttributeName] = Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.SubmeshMaterialAttribute.AttributeName] = Vim.G3d.Attributes.SubmeshMaterialAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.MaterialColorAttribute.AttributeName] = Vim.G3d.Attributes.MaterialColorAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.MaterialGlossinessAttribute.AttributeName] = Vim.G3d.Attributes.MaterialGlossinessAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.MaterialSmoothnessAttribute.AttributeName] = Vim.G3d.Attributes.MaterialSmoothnessAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.ShapeVertexAttribute.AttributeName] = Vim.G3d.Attributes.ShapeVertexAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.ShapeVertexOffsetAttribute.AttributeName] = Vim.G3d.Attributes.ShapeVertexOffsetAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.ShapeColorAttribute.AttributeName] = Vim.G3d.Attributes.ShapeColorAttribute.CreateAttributeReader(),
                [Vim.G3d.Attributes.ShapeWidthAttribute.AttributeName] = Vim.G3d.Attributes.ShapeWidthAttribute.CreateAttributeReader(),
            };

        // Named Attributes.

        public Vim.G3d.Attributes.CornersPerFaceAttribute CornersPerFaceAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.CornersPerFaceAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.CornersPerFaceAttribute : default;
            set => Attributes[Vim.G3d.Attributes.CornersPerFaceAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.VertexAttribute VertexAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.VertexAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.VertexAttribute : default;
            set => Attributes[Vim.G3d.Attributes.VertexAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.IndexAttribute IndexAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.IndexAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.IndexAttribute : default;
            set => Attributes[Vim.G3d.Attributes.IndexAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.InstanceTransformAttribute InstanceTransformAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.InstanceTransformAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.InstanceTransformAttribute : default;
            set => Attributes[Vim.G3d.Attributes.InstanceTransformAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.InstanceParentAttribute InstanceParentAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.InstanceParentAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.InstanceParentAttribute : default;
            set => Attributes[Vim.G3d.Attributes.InstanceParentAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.InstanceFlagsAttribute InstanceFlagsAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.InstanceFlagsAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.InstanceFlagsAttribute : default;
            set => Attributes[Vim.G3d.Attributes.InstanceFlagsAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.InstanceMeshAttribute InstanceMeshAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.InstanceMeshAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.InstanceMeshAttribute : default;
            set => Attributes[Vim.G3d.Attributes.InstanceMeshAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.MeshSubmeshOffsetAttribute MeshSubmeshOffsetAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.MeshSubmeshOffsetAttribute : default;
            set => Attributes[Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.SubmeshIndexOffsetAttribute SubmeshIndexOffsetAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.SubmeshIndexOffsetAttribute : default;
            set => Attributes[Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.SubmeshMaterialAttribute SubmeshMaterialAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.SubmeshMaterialAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.SubmeshMaterialAttribute : default;
            set => Attributes[Vim.G3d.Attributes.SubmeshMaterialAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.MaterialColorAttribute MaterialColorAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.MaterialColorAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.MaterialColorAttribute : default;
            set => Attributes[Vim.G3d.Attributes.MaterialColorAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.MaterialGlossinessAttribute MaterialGlossinessAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.MaterialGlossinessAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.MaterialGlossinessAttribute : default;
            set => Attributes[Vim.G3d.Attributes.MaterialGlossinessAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.MaterialSmoothnessAttribute MaterialSmoothnessAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.MaterialSmoothnessAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.MaterialSmoothnessAttribute : default;
            set => Attributes[Vim.G3d.Attributes.MaterialSmoothnessAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.ShapeVertexAttribute ShapeVertexAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.ShapeVertexAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.ShapeVertexAttribute : default;
            set => Attributes[Vim.G3d.Attributes.ShapeVertexAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.ShapeVertexOffsetAttribute ShapeVertexOffsetAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.ShapeVertexOffsetAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.ShapeVertexOffsetAttribute : default;
            set => Attributes[Vim.G3d.Attributes.ShapeVertexOffsetAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.ShapeColorAttribute ShapeColorAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.ShapeColorAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.ShapeColorAttribute : default;
            set => Attributes[Vim.G3d.Attributes.ShapeColorAttribute.AttributeName] = value as IAttribute;
        }

        public Vim.G3d.Attributes.ShapeWidthAttribute ShapeWidthAttribute
        {
            get => Attributes.TryGetValue(Vim.G3d.Attributes.ShapeWidthAttribute.AttributeName, out var attr) ? attr as Vim.G3d.Attributes.ShapeWidthAttribute : default;
            set => Attributes[Vim.G3d.Attributes.ShapeWidthAttribute.AttributeName] = value as IAttribute;
        }

        /// <inheritdoc/>
        public IAttribute GetAttribute(Type attributeType)
        {

            if (attributeType == typeof(Vim.G3d.Attributes.CornersPerFaceAttribute))
                return CornersPerFaceAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.VertexAttribute))
                return VertexAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.IndexAttribute))
                return IndexAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.InstanceTransformAttribute))
                return InstanceTransformAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.InstanceParentAttribute))
                return InstanceParentAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.InstanceFlagsAttribute))
                return InstanceFlagsAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.InstanceMeshAttribute))
                return InstanceMeshAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.MeshSubmeshOffsetAttribute))
                return MeshSubmeshOffsetAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.SubmeshIndexOffsetAttribute))
                return SubmeshIndexOffsetAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.SubmeshMaterialAttribute))
                return SubmeshMaterialAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.MaterialColorAttribute))
                return MaterialColorAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.MaterialGlossinessAttribute))
                return MaterialGlossinessAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.MaterialSmoothnessAttribute))
                return MaterialSmoothnessAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.ShapeVertexAttribute))
                return ShapeVertexAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.ShapeVertexOffsetAttribute))
                return ShapeVertexOffsetAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.ShapeColorAttribute))
                return ShapeColorAttribute;

            if (attributeType == typeof(Vim.G3d.Attributes.ShapeWidthAttribute))
                return ShapeWidthAttribute;

            throw new ArgumentException("Type {attributeType.ToString()} is not supported.");
        }

        public IAttribute MergeAttribute(string attributeName, IReadOnlyList<IAttributeCollection> otherCollections)
        {
            var collections = otherCollections.Prepend(this).ToArray();
            switch (attributeName)
            {

                case Vim.G3d.Attributes.CornersPerFaceAttribute.AttributeName:
                {
                    // Singleton Attribute (no merging)
                    return CornersPerFaceAttribute;
                }

                case Vim.G3d.Attributes.VertexAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.VertexAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.VertexAttribute, Vim.Math3d.Vector3>();
                }

                case Vim.G3d.Attributes.IndexAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.IndexAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.InstanceTransformAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.InstanceTransformAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.InstanceTransformAttribute, Vim.Math3d.Matrix4x4>();
                }

                case Vim.G3d.Attributes.InstanceParentAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.InstanceParentAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.InstanceFlagsAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.InstanceFlagsAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.InstanceFlagsAttribute, System.UInt16>();
                }

                case Vim.G3d.Attributes.InstanceMeshAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.InstanceMeshAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.MeshSubmeshOffsetAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.MeshSubmeshOffsetAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.SubmeshIndexOffsetAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.SubmeshIndexOffsetAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.SubmeshMaterialAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.SubmeshMaterialAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.MaterialColorAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.MaterialColorAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.MaterialColorAttribute, Vim.Math3d.Vector4>();
                }

                case Vim.G3d.Attributes.MaterialGlossinessAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.MaterialGlossinessAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.MaterialGlossinessAttribute, System.Single>();
                }

                case Vim.G3d.Attributes.MaterialSmoothnessAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.MaterialSmoothnessAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.MaterialSmoothnessAttribute, System.Single>();
                }

                case Vim.G3d.Attributes.ShapeVertexAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.ShapeVertexAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.ShapeVertexAttribute, Vim.Math3d.Vector3>();
                }

                case Vim.G3d.Attributes.ShapeVertexOffsetAttribute.AttributeName:
                {
                    // Index Attribute
                    return collections.GetIndexedAttributesOfType<Vim.G3d.Attributes.ShapeVertexOffsetAttribute>().MergeIndexAttributes();
                }

                case Vim.G3d.Attributes.ShapeColorAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.ShapeColorAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.ShapeColorAttribute, Vim.Math3d.Vector4>();
                }

                case Vim.G3d.Attributes.ShapeWidthAttribute.AttributeName:
                {
                    // Data Attribute
                    return collections.GetAttributesOfType<Vim.G3d.Attributes.ShapeWidthAttribute>().ToArray().MergeDataAttributes<Vim.G3d.Attributes.ShapeWidthAttribute, System.Single>();
                }

                default:
                    throw new ArgumentException(nameof(attributeName));
            }
        }

        public void Validate() 
        {
            // Ensure all the indices are either -1 or within the bounds of the attributes they are indexing into.

            {
                var maxIndex = GetAttribute(IndexAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < IndexAttribute.TypedData.Length; ++i)
                {
                    var index = IndexAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.IndexAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(InstanceParentAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < InstanceParentAttribute.TypedData.Length; ++i)
                {
                    var index = InstanceParentAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.InstanceParentAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(InstanceMeshAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < InstanceMeshAttribute.TypedData.Length; ++i)
                {
                    var index = InstanceMeshAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.InstanceMeshAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(MeshSubmeshOffsetAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < MeshSubmeshOffsetAttribute.TypedData.Length; ++i)
                {
                    var index = MeshSubmeshOffsetAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.MeshSubmeshOffsetAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(SubmeshIndexOffsetAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < SubmeshIndexOffsetAttribute.TypedData.Length; ++i)
                {
                    var index = SubmeshIndexOffsetAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.SubmeshIndexOffsetAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(SubmeshMaterialAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < SubmeshMaterialAttribute.TypedData.Length; ++i)
                {
                    var index = SubmeshMaterialAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.SubmeshMaterialAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }

            {
                var maxIndex = GetAttribute(ShapeVertexOffsetAttribute.IndexInto).Data.Length - 1;
                for (var i = 0; i < ShapeVertexOffsetAttribute.TypedData.Length; ++i)
                {
                    var index = ShapeVertexOffsetAttribute.TypedData[i];

                    if (index == -1)
                        continue; // no relation.

                    if (index < -1 || index > maxIndex)
                        throw new Exception($"Index out of range in Vim.G3d.Attributes.ShapeVertexOffsetAttribute at position {i}. Expected either -1 for no relation, or a maximum of {maxIndex} but got {index}");
                }
            }
        }
    }
}
