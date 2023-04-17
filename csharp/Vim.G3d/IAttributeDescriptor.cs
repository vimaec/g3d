namespace Vim.G3d
{
    public interface IAttributeDescriptor
    {
        /// <summary>
        /// The string representation of the IAttributeDescriptor.
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///      ~~~
        ///       |
        ///     "g3d" is the standard prefix.
        /// </code>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The second token in the AttributeDescriptor; designates the object to
        /// which the attribute is conceptually associated
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///          ~~~~~~~~
        /// </code>
        /// </summary>
        string Association { get; }

        /// <summary>
        /// The third token in the AttributeDescriptor; designates the semantic meaning of the data.
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///                   ~~~~~~~~~
        /// </code>
        /// </summary>
        string Semantic { get; }

        /// <summary>
        /// The fourth token in the AttributeDescriptor; designates the index in case
        /// the same Association:Semantic combination occurs more than once among the collection of attribute descriptors.
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///                             ^
        /// </code>
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The fifth token in the AttributeDescriptor; designates the data type which composes the buffer.
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///                               ~~~~~~~
        ///                                  |
        /// a transformation matrix is composed of float32 values
        /// </code>
        /// </summary>
        DataType DataType { get; }

        /// <summary>
        /// The sixth token in the AttributeDescriptor; designates arity, or the number of values which compose
        /// one semantic element.
        /// <code>
        /// ex: "g3d:instance:transform:0:float32:16"
        ///                                       ~~
        ///                                       |
        /// one transformation matrix is composed of 16 values
        /// </code>
        /// </summary>
        int DataArity { get; }

        /// <summary>
        /// The current collection of errors defined as bitwise flags
        /// </summary>
        AttributeDescriptorErrors Errors { get; }

        /// <summary>
        /// Returns true if the Errors is not AttributeDescriptorErrors.None.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// The size in bytes of the DataType.
        /// </summary>
        int DataTypeSize { get; }

        /// <summary>
        /// The size in bytes of one semantic element (DataTypeSize * DataArity)
        /// </summary>
        int DataElementSize { get; }
    }
}
