using System;
using System.Collections.Generic;
using System.Linq;

namespace Vim.G3d
{
    /// <summary>
    /// This is a helper class for constructing a G3D from individual attributes
    /// </summary>
    public class G3DBuilder
    {
        public Header Header { get; set; }
        public readonly List<BinaryAttribute> Attributes = new List<BinaryAttribute>();

        public G3D ToG3D()
            => new G3D(Attributes, Header);

        public void AddAttribute(BinaryAttribute attr)
            => Attributes.Add(attr);

        public void AddAttribute<T>(Attribute<T> attr) where T: struct
            => AddAttribute(attr._Attribute);

        public void AddIndices(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.Indices));

        /// <summary>
        /// Computes the face indices and the face sizes.
        /// Use this if dealing with a polygonal mesh. 
        /// </summary>
        public void AddIndicesByFace(IEnumerable<IEnumerable<int>> faces)
        {
            var faceSizes = new List<int>();
            var faceIndices = new List<int>();
            var indices = new List<int>();
            var currentIndex = 0;
            foreach (var f in faces)
            {
                var nFaceSize = 0;
                faceIndices.Add(currentIndex);
                foreach (var i in f)
                {
                    nFaceSize++;
                    indices.Add(currentIndex++);
                }
                faceSizes.Add(nFaceSize);
            }
            AddIndices(indices.ToArray());
            AddFaceIndices(faceIndices.ToArray());
            AddFaceSizes(faceSizes.ToArray());
        }

        public void AddVertices(float[] vertices)
            => AddAttribute(vertices.ToAttribute(CommonAttributes.Position));

        public void AddUV(float[] uvs)
            => AddAttribute(uvs.ToAttribute(CommonAttributes.UV));

        public void AddUVW(float[] uvws)
            => AddAttribute(uvws.ToAttribute(CommonAttributes.UVW));

        public void AddFaceNormals(float[] faceNormals)
            => AddAttribute(faceNormals.ToAttribute(CommonAttributes.FaceNormal));

        public void AddVertexNormals(float[] vertexNormals)
            => AddAttribute(vertexNormals.ToAttribute(CommonAttributes.VertexNormal));

        public void AddVertexColors(float[] vertexColors)
            => AddAttribute(vertexColors.ToAttribute(CommonAttributes.VertexColor));

        public void AddVertexColorsWithAlpha(float[] vertexColors)
            => AddAttribute(vertexColors.ToAttribute(CommonAttributes.VertexColorWithAlpha));

        public void AddTangent(float[] tangents)
            => AddAttribute(tangents.ToAttribute(CommonAttributes.Tangent));

        public void AddBitangent(float[] bitangents)
            => AddAttribute(bitangents.ToAttribute(CommonAttributes.Bitangent));

        public void AddTangentVector4(float[] tangents)
            => AddAttribute(tangents.ToAttribute(CommonAttributes.TangentVector4));

        public void AddFaceSizes(int[] sizes)
            => AddAttribute(sizes.ToAttribute(CommonAttributes.FaceSizes));

        public void AddFaceIndices(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.FaceIndices));

        public void AddGroupSizes(int[] sizes)
            => AddAttribute(sizes.ToAttribute(CommonAttributes.GroupSizes));

        public void AddGroupIndices(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.GroupIndices));
    }
}

