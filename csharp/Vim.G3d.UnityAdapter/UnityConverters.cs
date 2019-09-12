using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vim.G3d
{
    public static class UnityConverter
    {
        public static MeshTopology TopologyFromPointsPerFace(int n)
        {
            switch (n)
            {
                case 1: return MeshTopology.Points;
                case 2: return MeshTopology.Lines;
                case 3: return MeshTopology.Triangles;
                case 4: return MeshTopology.Quads;
                default: throw new Exception("Only points, lines, triangles, and quads are supported");
            }
        }
        
        public static int PointsPerFaceFromTopology(MeshTopology topo)
        {
            switch (topo)
            {
                case MeshTopology.Points: return 1;
                case MeshTopology.Lines: return 2;
                case MeshTopology.Triangles: return 3;
                case MeshTopology.Quads: return 4;
                default: throw new Exception($"ObjectFaceSize {topo} not supported");
            }
        }

        public static Vector2[] ToUnityUV(this Attribute<float> attr)
            => attr?.CheckArityAndAssociation(2, Association.assoc_vertex)?.CastData<Vector2>().ToArray();

        public static Color[] ToUnityVertexColor(this Attribute<float> attr)
            => attr?.CheckArityAndAssociation(4, Association.assoc_vertex)?.CastData<Color>().ToArray();

        public static Vector3[] ToUnityNormal(this Attribute<float> attr)
            => attr?.CheckArityAndAssociation(3, Association.assoc_vertex)?.CastData<Vector3>().ToArray();

        public static Vector4[] ToUnityTangent(this Attribute<float> attr)
            => attr?.CheckArityAndAssociation(4, Association.assoc_vertex)?.CastData<Vector4>().ToArray();

        public static Mesh CopyTo(this G3D g3d, Mesh mesh)
        {
            mesh.vertices = g3d.Vertices.CastData<Vector3>().ToArray();

            mesh.uv = g3d.UV.ElementAtOrDefault(0).ToUnityUV();
            mesh.uv2 = g3d.UV.ElementAtOrDefault(1).ToUnityUV();
            mesh.uv3 = g3d.UV.ElementAtOrDefault(2).ToUnityUV();
            mesh.uv4 = g3d.UV.ElementAtOrDefault(3).ToUnityUV();
            mesh.uv5 = g3d.UV.ElementAtOrDefault(4).ToUnityUV();
            mesh.uv6 = g3d.UV.ElementAtOrDefault(5).ToUnityUV();
            mesh.uv7 = g3d.UV.ElementAtOrDefault(6).ToUnityUV();
            mesh.uv8 = g3d.UV.ElementAtOrDefault(7).ToUnityUV();

            mesh.colors = g3d.VertexColor.ElementAtOrDefault(0)?.ToUnityVertexColor();
            mesh.normals = g3d.VertexNormal.ToUnityNormal();
            mesh.tangents = g3d.Tangents.ToUnityTangent();

            if (g3d.NumGroups > 0)
            {
                foreach (var grp in g3d.Groups)
                {
                    var topo = TopologyFromPointsPerFace(grp.CornersPerFace);
                    mesh.SetIndices(grp.Indices.ToArray(), topo, grp.Id);
                }
            }
            else
            {
                var topo = TopologyFromPointsPerFace(g3d.CornersPerFace);
                mesh.SetIndices(g3d.Indices.Data.ToArray(), topo, 0);
            }
            return mesh;
        }

        public static G3DBuilder AddUnityUV(this G3DBuilder gb, Vector2[] vectors)
            => vectors == null ? gb : gb.AddAttribute(vectors.ToBinaryAttribute(CommonAttributes.UV));

        public static G3DBuilder AddUnityVertices(this G3DBuilder gb, Vector3[] vectors)
            => vectors == null ? gb : gb.AddAttribute(vectors.ToBinaryAttribute(CommonAttributes.Position));

        public static G3DBuilder AddUnityColors(this G3DBuilder gb, Color[] colors)
            => colors == null ? gb : gb.AddAttribute(colors.ToBinaryAttribute(CommonAttributes.VertexColorWithAlpha));

        public static G3DBuilder AddUnityNormals(this G3DBuilder gb, Vector3[] normals)
            => normals == null ? gb : gb.AddAttribute(normals.ToBinaryAttribute(CommonAttributes.VertexNormal));

        public static G3DBuilder AddUnityTangent(this G3DBuilder gb, Vector4[] tangents)
            => tangents == null ? gb : gb.AddAttribute(tangents.ToBinaryAttribute(CommonAttributes.Tangent4));

        public static G3D ToG3D(this Mesh mesh)
        {
            var g = new G3DBuilder();

            // NOTE: if the any of the UV channels are null, followed by a non-null channel, then the UV channels will get reindexed 
            // during reimport of the G3D. This would be quite rare. 
            g.AddUnityUV(mesh.uv);
            g.AddUnityUV(mesh.uv2);
            g.AddUnityUV(mesh.uv3);
            g.AddUnityUV(mesh.uv4);
            g.AddUnityUV(mesh.uv5);
            g.AddUnityUV(mesh.uv6);
            g.AddUnityUV(mesh.uv7);
            g.AddUnityUV(mesh.uv8);

            g.AddUnityVertices(mesh.vertices);
            g.AddUnityNormals(mesh.normals);
            g.AddUnityTangent(mesh.tangents);
            g.AddUnityColors(mesh.colors);

            var indices = new List<int>();
            var offsets = new List<int>();
            var offset = 0;
            for (var i = 0; i < mesh.subMeshCount; ++i)
            {
                offsets.Add(offset);
                var subIndices = mesh.GetIndices(i);
                indices.AddRange(subIndices);
                offset += subIndices.Length;
            }

            g.AddGroupIndexOffsets(offsets.ToArray());
            g.AddIndices(indices.ToArray());

            return g.ToG3D();
        }
    }
}
