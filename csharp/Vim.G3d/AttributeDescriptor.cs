using System;
using System.Diagnostics;

namespace Vim.G3d
{
    /// <summary>
    /// Provides information about identifying the role and parsing the data within an attribute data buffer.
    /// This is encoded using a string in a particular URN form. 
    /// </summary>
    public class AttributeDescriptor
    {
        public AssociationEnum Association { get; }
        public SemanticEnum Semantic { get; }
        public DataTypeEnum DataType { get; }
        public int DataArity { get; }

        public int DataElementSize { get; }
        public int DataTypeSize { get; }

        public AttributeDescriptor(AssociationEnum association, SemanticEnum semantic, DataTypeEnum dataType, int dataArity)
        {
            Association = association;
            Semantic = semantic;
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
            => $"g3d:{SemanticString}:{AssociationString}:{DataTypeString}:{DataArity}";

        /// <summary>
        /// Parses a URN representation of the attribute descriptor to generate an actual attribute descriptor 
        /// </summary>
        public static AttributeDescriptor Parse(string urn)
        {
            var vals = urn.Split(':');
            if (vals.Length != 5) throw new Exception("Expected 5 parts to the attribute descriptor URN");
            if (vals[0] != "g3d") throw new Exception("First part of URN must be g3d");
            return new AttributeDescriptor(
                ParseAssociation(vals[1]),
                ParseSemantic(vals[2]),
                ParseDataType(vals[3]),
                int.Parse(vals[4])
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
                case DataTypeEnum.dt_uint8: return 1;
                case DataTypeEnum.dt_uint16: return 2;
                case DataTypeEnum.dt_uint32: return 4;
                case DataTypeEnum.dt_uint64: return 8;
                case DataTypeEnum.dt_float32: return 4;
                case DataTypeEnum.dt_float64: return 8;
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

}