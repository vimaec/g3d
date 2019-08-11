using System;
using System.Diagnostics;

namespace Vim.G3D
{
    /// <summary>
    /// Provides information about identifying the role and parsing the data within an attribute data buffer.
    /// This is encoded using a string in a particular URN form. 
    /// </summary>
    public class AttributeDescriptor
    {
        public AssociationEnum Association { get; }
        public SemanticEnum Semantic { get; }
        public int SemanticIndex { get; }
        public DataTypeEnum DataType { get; }
        public int DataArity { get; }

        public int DataElementSize { get; }
        public int DataTypeSize { get; }

        public AttributeDescriptor(AssociationEnum association, SemanticEnum semantic, int semIndex, DataTypeEnum dataType, int dataArity)
        {
            Association = association;
            Semantic = semantic;
            SemanticIndex = semIndex;
            DataType = dataType;
            DataArity = dataArity;

            DataTypeSize = GetDataTypeSize(DataType);
            DataElementSize = DataTypeSize * DataArity;

            Debug.Assert(Validate());
        }

        /// <summary>
        /// Generates a URN representation of the attribute descriptor
        /// </summary>
        public override string ToString()
            => $"g3d:{SemanticString}:{AssociationString}:{SemanticIndex}:{DataTypeString}:{DataArity}";

        /// <summary>
        /// Parses a URN representation of the attribute descriptor to generate an actual attribute descriptor 
        /// </summary>
        public static AttributeDescriptor Parse(string urn)
        {
            var vals = urn.Split(':');
            if (vals.Length != 6) throw new Exception("Expected 6 parts to the attribute descriptor URN");
            if (vals[0] != "g3d") throw new Exception("First part of URN must be g3d");
            return new AttributeDescriptor(
                ParseAssociation(vals[1]),
                ParseSemantic(vals[2]),
                int.Parse(vals[3]),
                ParseDataType(vals[4]),
                int.Parse(vals[5])
            );
        }

        public bool Validate()
        {
            var urn = ToString();
            var tmp = Parse(urn);
            if (!Equals(tmp))
                throw new Exception("Invalid attribute descriptor (or internal error in the parsing/string conversion");
            return true;
        }

        public bool Equals(AttributeDescriptor other)
            => ToString() == other.ToString();

        public static int GetDataTypeSize(DataTypeEnum dt)
        {
            switch (dt)
            {
                case DataTypeEnum.dt_int8: return 1;
                case DataTypeEnum.dt_int16: return 2;
                case DataTypeEnum.dt_int32: return 4;
                case DataTypeEnum.dt_int64: return 8;
                case DataTypeEnum.dt_float32: return 4;
                case DataTypeEnum.dt_float64: return 8;
                case DataTypeEnum.dt_string: return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }

        public string AssociationString
            => Association.ToString().Substring("assoc_".Length);

        public static AssociationEnum ParseAssociation(string s)
            => (AssociationEnum)Enum.Parse(typeof(AssociationEnum), "assoc_" + s);

        public string SemanticString
            => Semantic.ToString().Substring("sem_".Length) ?? "unknown";

        public static SemanticEnum ParseSemantic(string s)
            => (SemanticEnum)Enum.Parse(typeof(SemanticEnum), "sem_" + s);

        public string DataTypeString
            => DataType.ToString()?.Substring("dt_".Length) ?? null;

        public static DataTypeEnum ParseDataType(string s)
            => (DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), "dt_" + s);
    }

    /// <summary>
    /// The different types of data types that can be used as elements.
    /// </summary> 
    public enum DataTypeEnum
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
    /// What element of a mesh each attribute is associated with.  
    /// </summary>  
    public enum AssociationEnum
    {
        assoc_none,     // no association
        assoc_vertex,   // vertex data 
        assoc_face,     // face data
        assoc_corner,   // corner data (each face has n corners)
        assoc_edge,     // half-edge data (each face consists of n half-edges, one per corner)
        assoc_group,    // face-group (each face-group is a contiguous series of faces, and can be thought of as a surface or sub-mesh)
        assoc_object,   // data associated with the entire object 
    };

    /// <summary>
    /// The semantic (role or meaning) of the attribute. These are commonly defined roles in geometry processing applications.    
    /// </summary>
    public enum SemanticEnum
    {
        sem_unknown,       // no known attribute type
        sem_vertex,        // vertex buffer 
        sem_index,         // index buffer
        sem_offset,        // an offset into the index buffer (used with groups)
        sem_vertexoffset,  // the offset into the vertex buffer (used only with groups, and must have offset)
        sem_normal,        // computed normal information (per face, group, corner, or vertex)
        sem_binormal,      // computed binormal information 
        sem_tangent,       // computed tangent information 
        sem_materialid,    // material id
        sem_visibility,    // visibility data (e.g. 
        sem_count,         // number of indices per face or group
        sem_uv,            // UV (sometimes more than 1, e.g. Unity supports up to 8)
        sem_color,         // usually vertex color, but could be edge color as well
        sem_smoothing,     // identifies smoothing groups (e.g. ala 3ds Max and OBJ files)
        sem_weight,        // in 3ds Max this is called selection 
        sem_mapchannel,    // 3ds Max map channel (assoc of none => map verts, assoc of corner => map faces)
        sem_id,            // used to identify what object each face part came from 
        sem_joint,         // used to identify what a joint a skin is associated with 
        sem_user,          // identifies user specific data (in 3ds Max this could be "per-vertex-data")
    };
}