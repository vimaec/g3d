using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Vim.LinqArray;

namespace Vim.G3d
{
    public static class G3dUnityAdapater
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

        public static Vector3[] ToUnityVector3Array(this IArray<Math3d.Vector3> vectors)
            => vectors.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();

        public static Vector2[] ToUnityVector2Array(this IArray<Math3d.Vector2> vectors)
            => vectors.Select(v => new Vector2(v.X, v.Y)).ToArray();

        public static Vector4[] ToUnityVector4Array(this IArray<Math3d.Vector4> vectors)
            => vectors.Select(v => new Vector4(v.X, v.Y, v.Z, v.W)).ToArray();

        public static Color[] ToUnityColorArray(this IArray<Math3d.Vector4> vectors)
            => vectors.Select(v => new Color(v.X, v.Y, v.Z, v.W)).ToArray();

        public static Mesh ToMesh(this G3D g3d)
            => g3d.CopyTo(new Mesh());

        public static Mesh ToMesh(this TextAsset asset)
            => asset.ToG3D().ToMesh();

        public static Mesh CopyTo(this G3D g3d, Mesh mesh)
        {
            mesh.vertices = g3d.Vertices.ToUnityVector3Array();

            mesh.uv = g3d.AllVertexUvs.ElementAtOrDefault(0).ToUnityVector2Array();
            mesh.uv2 = g3d.AllVertexUvs.ElementAtOrDefault(1).ToUnityVector2Array();
            mesh.uv3 = g3d.AllVertexUvs.ElementAtOrDefault(2).ToUnityVector2Array();
            mesh.uv4 = g3d.AllVertexUvs.ElementAtOrDefault(3).ToUnityVector2Array();
            mesh.uv5 = g3d.AllVertexUvs.ElementAtOrDefault(4).ToUnityVector2Array();
            mesh.uv6 = g3d.AllVertexUvs.ElementAtOrDefault(5).ToUnityVector2Array();
            mesh.uv7 = g3d.AllVertexUvs.ElementAtOrDefault(6).ToUnityVector2Array();
            mesh.uv8 = g3d.AllVertexUvs.ElementAtOrDefault(7).ToUnityVector2Array();

            mesh.colors = g3d.AllVertexColors.ElementAtOrDefault(0)?.ToUnityColorArray();
            mesh.normals = g3d.VertexNormals.ToUnityVector3Array();

            mesh.tangents = g3d.VertexTangents.ToUnityVector4Array();
            mesh.indexFormat = IndexFormat.UInt32;

            mesh.SetIndices(g3d.Indices.ToArray(), TopologyFromPointsPerFace(g3d.NumCornersPerFace), 0);
            
            if (mesh.normals == null || mesh.normals.Length == 0)
                mesh.RecalculateNormals();

            return mesh;
        }

        public static G3DBuilder AddUnityUV(this G3DBuilder gb, Vector2[] vectors, int index)
        {
            if (vectors?.Length == 0)
                return gb;
            
            var data = vectors.Select(v => new Math3d.Vector2(v.x, v.y)).ToIArray();
            var attrDescr = AttributeDescriptor.Parse(CommonAttributes.VertexUv).SetIndex(index);

            return gb.Add(new GeometryAttribute<Math3d.Vector2>(data, attrDescr));
        }

        public static G3DBuilder AddUnityVertices(this G3DBuilder gb, Vector3[] vectors)
        {
            if (vectors?.Length == 0)
                return gb;

            var data = vectors.Select(v => new Math3d.Vector3(v.x, v.y, v.z)).ToIArray();
            var attrDescr = AttributeDescriptor.Parse(CommonAttributes.Position);

            return gb.Add(new GeometryAttribute<Math3d.Vector3>(data, attrDescr));
        }

        public static G3DBuilder AddUnityColors(this G3DBuilder gb, Color[] colors)
        {
            if (colors?.Length == 0)
                return gb;

            var data = colors.Select(c => new Math3d.Vector4(c.r, c.g, c.b, c.a)).ToIArray();
            var attrDescr = AttributeDescriptor.Parse(CommonAttributes.VertexColor);

            return gb.Add(new GeometryAttribute<Math3d.Vector4>(data, attrDescr));
        }

        public static G3DBuilder AddUnityNormals(this G3DBuilder gb, Vector3[] normals)
        {
            if (normals?.Length == 0)
                return gb;

            var data = normals.Select(v => new Math3d.Vector3(v.x, v.y, v.z)).ToIArray();
            var attrDescr = AttributeDescriptor.Parse(CommonAttributes.VertexNormal);

            return gb.Add(new GeometryAttribute<Math3d.Vector3>(data, attrDescr));
        }

        public static G3DBuilder AddUnityTangent(this G3DBuilder gb, Vector4[] tangents)
        {
            if (tangents?.Length == 0)
                return gb;

            var data = tangents.Select(v => new Math3d.Vector4(v.x, v.y, v.z, v.w)).ToIArray();
            var attrDescr = AttributeDescriptor.Parse(CommonAttributes.VertexTangent);

            return gb.Add(new GeometryAttribute<Math3d.Vector4>(data, attrDescr));
        }

        public static G3D ToG3D(this Mesh mesh)
        {
            var g = new G3DBuilder();

            // NOTE: if the any of the UV channels are null, followed by a non-null channel, then the UV channels will get reindexed 
            // during reimport of the G3D. This would be quite rare. 
            var uvIndex = 0;
            g.AddUnityUV(mesh.uv, uvIndex++);
            g.AddUnityUV(mesh.uv2, uvIndex++);
            g.AddUnityUV(mesh.uv3, uvIndex++);
            g.AddUnityUV(mesh.uv4, uvIndex++);
            g.AddUnityUV(mesh.uv5, uvIndex++);
            g.AddUnityUV(mesh.uv6, uvIndex++);
            g.AddUnityUV(mesh.uv7, uvIndex++);
            g.AddUnityUV(mesh.uv8, uvIndex++);

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

            g.AddIndices(indices.ToArray());

            return g.ToG3D();
        }

        public static void WriteToFile(this Mesh mesh, string filePath)
            => mesh.ToG3D().Write(filePath);

        public static void WriteToFile(this Mesh mesh, Stream stream)
            => mesh.ToG3D().Write(stream);

        public static G3D ToG3D(this TextAsset asset)
            => throw new NotImplementedException(); // TODO: Restore this
            //=> asset.bytes.ToG3D();

        public static G3D LoadG3DAsset(string assetName)
            => Resources.Load<TextAsset>(assetName).ToG3D();
    }
}
