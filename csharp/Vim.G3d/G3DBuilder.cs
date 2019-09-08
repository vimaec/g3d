using System.Collections.Generic;

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

        public G3DBuilder AddAttribute(BinaryAttribute attr)
        {
            Attributes.Add(attr);
            return this;
        }

        public G3DBuilder AddAttribute<T>(Attribute<T> attr) where T: struct
            => AddAttribute(attr._Attribute);

        public G3DBuilder AddIndices(int[] indices)
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

        public G3DBuilder SetObjectFaceSize(int n)
            => AddAttribute(new[] {n}.ToAttribute(CommonAttributes.ObjectFaceSize));

        public G3DBuilder SetGroupFaceSize(int[] faceSizes)
            => AddAttribute(faceSizes.ToAttribute(CommonAttributes.GroupFaceSize));

        public G3DBuilder AddVertices(float[] vertices)
            => AddAttribute(vertices.ToAttribute(CommonAttributes.Position));

        public G3DBuilder AddUV(float[] uvs)
            => AddAttribute(uvs.ToAttribute(CommonAttributes.UV));

        public G3DBuilder AddUVW(float[] uvws)
            => AddAttribute(uvws.ToAttribute(CommonAttributes.UVW));

        public G3DBuilder AddFaceNormals(float[] faceNormals)
            => AddAttribute(faceNormals.ToAttribute(CommonAttributes.FaceNormal));

        public G3DBuilder AddVertexNormals(float[] vertexNormals)
            => AddAttribute(vertexNormals.ToAttribute(CommonAttributes.VertexNormal));

        public G3DBuilder AddVertexColors(float[] vertexColors)
            => AddAttribute(vertexColors.ToAttribute(CommonAttributes.VertexColor));

        public G3DBuilder AddVertexColorsWithAlpha(float[] vertexColors)
            => AddAttribute(vertexColors.ToAttribute(CommonAttributes.VertexColorWithAlpha));

        public G3DBuilder AddBitangent(float[] tangents)
            => AddAttribute(tangents.ToAttribute(CommonAttributes.Bitangent));

        public G3DBuilder AddTangent3(float[] tangents)
            => AddAttribute(tangents.ToAttribute(CommonAttributes.Tangent3));

        public G3DBuilder AddTangent4(float[] tangents)
            => AddAttribute(tangents.ToAttribute(CommonAttributes.Tangent4));

        public G3DBuilder AddFaceSizes(int[] sizes)
            => AddAttribute(sizes.ToAttribute(CommonAttributes.FaceSizes));

        public G3DBuilder AddFaceIndices(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.FaceIndices));

        public G3DBuilder AddGroupIndexOffsets(int[] indices)
            => AddAttribute(indices.ToAttribute(CommonAttributes.GroupIndices));

        public G3DBuilder AddMaterialIds(int[] materialIds)
            => AddAttribute(materialIds.ToAttribute(CommonAttributes.MaterialIds));
    }
}

