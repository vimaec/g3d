using System;
using static Vim.G3d.Constants;

namespace Vim.G3d
{
    public class AttributeDescriptor : IAttributeDescriptor
    {
        /// <inhertdoc/>
        public string Name
            => string.Join(Separator, G3dPrefix, Association, Semantic, IndexStr, DataType, DataArity);

        /// <inhertdoc/>
        public string Association { get; }

        /// <inhertdoc/>
        public string Semantic { get; }

        /// <inhertdoc/>
        public int Index { get; }
        private readonly string IndexStr;

        /// <inhertdoc/>
        public DataType DataType { get; }
        public readonly string DataTypeStr;

        /// <inhertdoc/>
        public int DataArity { get; }
        public readonly string DataArityStr;

        /// <inhertdoc/>
        public AttributeDescriptorErrors Errors { get; }

        /// <inhertdoc/>
        public bool HasErrors
            => Errors != AttributeDescriptorErrors.None;

        /// <summary>
        /// Returns true if the Errors contains a data type error.
        /// </summary>
        public bool HasDataTypeError
            => (Errors & AttributeDescriptorErrors.DataTypeError) == AttributeDescriptorErrors.DataTypeError;

        /// <summary>
        /// Returns true if the Errors contains a data arity error.
        /// </summary>
        public bool HasDataArityError
            => (Errors & AttributeDescriptorErrors.DataArityError) == AttributeDescriptorErrors.DataArityError;

        /// <inhertdoc/>
        public int DataTypeSize { get; }

        /// <inhertdoc/>
        public int DataElementSize { get; }

        public static (int dataTypeSize, int dataElementSize) GetDataSizes(DataType dataType, int dataArity)
        {
            var dataTypeSize = dataType.GetDataTypeSize();
            return (dataTypeSize, dataTypeSize * dataArity);
        }

        /// <summary>
        /// Constructor.
        /// <code>
        ///  association   semantic   dataType
        ///         |         |          |
        ///      ~~~~~~~~ ~~~~~~~~~   ~~~~~~~
        /// "g3d:instance:transform:0:float32:16"
        ///                         ^         ~~
        ///                         |         |
        ///                       index    dataArity
        /// </code>
        /// </summary>
        public AttributeDescriptor(
            string association,
            string semantic,
            int index,
            DataType dataType,
            int dataArity)
        {
            if (string.IsNullOrWhiteSpace(association))
                throw new ArgumentException($"The association cannot be null or whitespace.");

            if (association.Contains(Separator))
                throw new ArgumentException($"The association cannot contain a '{Separator}' character");

            if (dataType == DataType.unknown)
                throw new ArgumentException($"The data type cannot be '{DataType.unknown}'.");

            if (string.IsNullOrWhiteSpace(semantic))
                throw new ArgumentException($"The semantic cannot be null or whitespace.");

            if (semantic.Contains(Separator))
                throw new ArgumentException($"The semantic must not contain a '{Separator}' character");

            Association = association;
            Semantic = semantic;
            Index = index;
            IndexStr = Index.ToString();
            DataType = dataType;
            DataTypeStr = DataType.ToString("G");
            DataArity = dataArity;
            DataArityStr = DataArity.ToString();
            (DataTypeSize, DataElementSize) = GetDataSizes(DataType, DataArity);
        }

        /// <summary>
        /// Constructor. Parses an input string of the form: "g3d:instance:transform:0:float32:16".
        /// </summary>
        public AttributeDescriptor(string str)
        {
            var tokens = str.Split(SeparatorChar);
            
            if (tokens.Length != 6)
            {
                Errors = AttributeDescriptorErrors.UnexpectedNumberOfTokens;
                return;
            }

            if (tokens[0] != G3dPrefix)
            {
                Errors = AttributeDescriptorErrors.PrefixError;
                return;
            }

            Association = tokens[1];
            if (string.IsNullOrWhiteSpace(Association))
                Errors |= AttributeDescriptorErrors.AssociationError; // do not return; there may be more errors.

            Semantic = tokens[2];
            if (string.IsNullOrWhiteSpace(Semantic))
                Errors |= AttributeDescriptorErrors.SemanticError; // do not return; there may be more errors.

            IndexStr = tokens[3];
            if (!int.TryParse(IndexStr, out var index))
                Errors |= AttributeDescriptorErrors.IndexError; // do not return; there may be more errors.
            Index = index;

            DataTypeStr = tokens[4];
            if (!Enum.TryParse(DataTypeStr, out DataType dataType))
                dataType = DataType.unknown;

            DataType = dataType;
            if (DataType == DataType.unknown)
                Errors |= AttributeDescriptorErrors.DataTypeError; // do not return; there may be more errors.

            DataArityStr = tokens[5];
            if (!int.TryParse(DataArityStr, out var dataArity))
                Errors |= AttributeDescriptorErrors.DataArityError;
            DataArity = dataArity;

            if (!HasDataTypeError && !HasDataArityError)
            {
                (DataTypeSize, DataElementSize) = GetDataSizes(DataType, DataArity);
            }
        }

        /// <summary>
        /// Returns the string representation of the attribute descriptor, in the form "g3d:instance:transform:0:float32:16"
        /// </summary>
        public override string ToString()
            => Name;

        /// <summary>
        /// Attempts to parse the given string as an AttributeDescriptor.
        /// </summary>
        public static bool TryParse(string str, out AttributeDescriptor attributeDescriptor)
        {
            attributeDescriptor = null;

            try
            {
                attributeDescriptor = new AttributeDescriptor(str);
            }
            catch
            {
                return false;
            }

            return !attributeDescriptor.HasErrors;
        }
    }
}
