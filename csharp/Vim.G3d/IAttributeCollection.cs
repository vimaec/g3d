using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vim.BFast;

namespace Vim.G3d
{
    /// <summary>
    /// A function which reads an attribute from the given stream and size.
    /// </summary>
    public delegate IAttribute AttributeReader(Stream stream, long sizeInBytes);

    /// <summary>
    /// A collection of attributes and readers which can be used to deserialize attributes from a stream.<br/>
    /// <br/>
    /// A class may implement this interface to define the specialized set of attributes and attribute readers for 
    /// a given context.<br/>
    /// <br/>
    /// For example, the geometry and instance information in a VIM file is defined in a class named VimAttributeCollection.
    /// </summary>
    public interface IAttributeCollection
    {
        /// <summary>
        /// Returns the set of attribute names supported by the AttributeCollection.
        /// </summary>
        IEnumerable<string> AttributeNames { get; }

        /// <summary>
        /// A mapping from attribute name to its corresponding attribute.<br/>
        /// This is populated when reading attributes from a stream.
        /// </summary>
        IDictionary<string, IAttribute> Attributes { get; }

        /// <summary>
        /// A mapping from attribute name to its corresponding attribute reader.
        /// </summary>
        IDictionary<string, AttributeReader> AttributeReaders { get; }

        /// <summary>
        /// Validates the attribute collection. May throw an exception if the collection is invalid.
        /// </summary>
        void Validate();

        /// <summary>
        /// Returns the attribute corresponding to the given type.
        /// </summary>
        IAttribute GetAttribute(Type attributeType);

        /// <summary>
        /// Merges the attribute with the given name with any other matching attributes in the other collections.
        /// </summary>
        //IAttribute MergeAttribute<T>(string attributeName, IReadOnlyList<T> otherCollections) where T : IAttributeCollection;
        IAttribute MergeAttribute(string attributeName, IReadOnlyList<IAttributeCollection> otherCollections);
    }

    /// <summary>
    /// Extension functions and helpers for attribute collections
    /// </summary>
    public static class AttributeCollectionExtensions
    {
        /// <summary>
        /// Reads the attribute buffer and stores it among the Buffers.
        /// </summary>
        public static bool ReadAttribute(
            this IAttributeCollection attributeCollection,
            Stream stream,
            string name,
            long sizeInBytes)
        {
            if (name == null || !attributeCollection.AttributeReaders.TryGetValue(name, out var readAttribute))
                return stream.ReadFailure(sizeInBytes);

            attributeCollection.Attributes[name] = readAttribute(stream, sizeInBytes);

            return true;
        }

        /// <summary>
        /// Creates an attribute reader
        /// </summary>
        public static AttributeReader CreateAttributeReader<T, U>()
            where U : unmanaged
            where T : IAttribute<U>, new()
            => (stream, sizeInBytes) =>
            {
                var attributeBuffer = new T();
                var attributeDescriptor = attributeBuffer.AttributeDescriptor;
                var numElements = sizeInBytes / attributeDescriptor.DataElementSize;

                if (sizeInBytes % attributeDescriptor.DataElementSize != 0 ||
                    numElements > int.MaxValue)
                {
                    stream.ReadFailure(sizeInBytes); // consume the stream
                    return attributeBuffer;
                }

                var data = stream.ReadArray<U>((int)numElements);
                attributeBuffer.TypedData = data;

                return attributeBuffer;
            };

        public static IEnumerable<TAttr> GetAttributesOfType<TAttr>(
            this IReadOnlyList<IAttributeCollection> collections)
            where TAttr : IAttribute
            => collections
                .Select(c => c.Attributes.Values.OfType<TAttr>().FirstOrDefault())
                .Where(a => a != null);

        public static IEnumerable<(TAttr Attribute, int IndexedCount)> GetIndexedAttributesOfType<TAttr>(
            this IReadOnlyList<IAttributeCollection> collections)
            where TAttr : IAttribute<int>
            => collections
                .Select(c =>
                {
                    var attr = c.Attributes.Values.OfType<TAttr>().FirstOrDefault();
                    if (attr == null || attr.IndexInto == null)
                        return (attr, 0);

                    var indexedAttr = c.GetAttribute(attr.IndexInto);
                    return (attr, indexedAttr.Data.Length);
                })
                .Where(t => t.attr != null);

        public static T Merge<T>(this IReadOnlyList<T> collections)
            where T: IAttributeCollection, new()
        {
            if (collections == null)
                return new T();

            if (collections.Count == 1)
                return collections[0];

            // More than one collection; the first collection dictates the attributes to merge.
            var @base = collections.First();
            
            var others = new List<IAttributeCollection>();
            for (var i = 1; i < collections.Count; ++i)
            {
                others.Add(collections[i]);
            }

            var result = new T();
            
            foreach (var item in @base.Attributes)
            {
                var name = item.Key;

                var merged = @base.MergeAttribute(name, others);

                result.Attributes[name] = merged;
            }

            return result;
        }

        /// <summary>
        /// Merges the given list of G3ds.<br/>
        /// - If there is no g3d, returns a new empty g3d.<br/>
        /// - If there is only one g3d, returns that g3d.
        /// </summary>
        public static G3d<T> Merge<T>(this IReadOnlyList<G3d<T>> g3ds)
            where T : IAttributeCollection, new()
        {
            switch (g3ds.Count)
            {
                case 0:
                    return new G3d<T>();
                case 1:
                    return g3ds.First();
                default:
                    return new G3d<T>(g3ds.Select(g => g.AttributeCollection).ToArray().Merge());
            }
        }
    }
}
