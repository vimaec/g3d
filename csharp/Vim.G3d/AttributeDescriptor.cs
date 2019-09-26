using System;

namespace Vim.G3d
{
    /// <summary>
    /// Provides information about identifying the role and parsing the data within an attribute data buffer.
    /// This is encoded using a string in a particular URN form. 
    /// </summary>
    public class AttributeDescriptor
    {
        public Association Association { get; }
        public Semantic Semantic { get; }
        public DataType DataType { get; }
        public int DataArity { get; }

        public int DataElementSize { get; }
        public int DataTypeSize { get; }

        public AttributeDescriptor(Association association, Semantic semantic, DataType dataType, int dataArity)
        {
            Association = association;
            Semantic = semantic;
            DataType = dataType;
            DataArity = dataArity;

            DataTypeSize = GetDataTypeSize(DataType);
            DataElementSize = DataTypeSize * DataArity;
        }

        /// <summary>
        /// Generates a URN representation of the attribute descriptor
        /// </summary>
        public override string ToString()
            => $"g3d:{AssociationString}:{SemanticString}:{DataTypeString}:{DataArity}";

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

        public static int GetDataTypeSize(DataType dt)
        {
            switch (dt)
            {
                case DataType.dt_int8: return 1;
                case DataType.dt_int16: return 2;
                case DataType.dt_int32: return 4;
                case DataType.dt_int64: return 8;
                case DataType.dt_float32: return 4;
                case DataType.dt_float64: return 8;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dt), dt, null);
            }
        }

        public string AssociationString
            => Association.ToString().Substring("assoc_".Length);

        public static Association ParseAssociation(string s)
            => (Association)Enum.Parse(typeof(Association), "assoc_" + s);

        public string SemanticString
            => Semantic.ToString().Substring("sem_".Length) ?? "unknown";

        public static Semantic ParseSemantic(string s)
            => (Semantic)Enum.Parse(typeof(Semantic), "sem_" + s);

        public string DataTypeString
            => DataType.ToString()?.Substring("dt_".Length) ?? null;

        public static DataType ParseDataType(string s)
            => (DataType)Enum.Parse(typeof(DataType), "dt_" + s);
    }

}