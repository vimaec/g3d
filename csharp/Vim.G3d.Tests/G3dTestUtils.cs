using Assimp;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Vim.G3d.AssimpWrapper;
using Vim.LinqArray;

namespace Vim.G3d.Tests
{
    public static class G3dTestUtils
    {
        public static void OutputSceneStats(Scene scene)
            => Console.WriteLine(
$@"    #animations = {scene.AnimationCount}
    #cameras = {scene.CameraCount}
    #lights = {scene.LightCount}
    #materials = {scene.MaterialCount}
    #meshes = {scene.MeshCount}
    #nodes = {scene.GetNodes().Count()}
    #textures = {scene.TextureCount}");

        // TODO: merge all of the meshes using the transform. 

        public static void OutputMeshStats(Mesh mesh)
            => Console.WriteLine(
                $@"
    mesh  {mesh.Name}
    #faces = {mesh.FaceCount}
    #vertices = {mesh.VertexCount}
    #normals = {mesh.Normals?.Count ?? 0}
    #texture coordinate chanels = {mesh.TextureCoordinateChannelCount}
    #vertex color chanels = {mesh.VertexColorChannelCount}
    #bones = {mesh.BoneCount}
    #tangents = {mesh.Tangents?.Count}
    #bitangents = {mesh.BiTangents?.Count}");

        public static T TimeLoadingFile<T>(string fileName, Func<string, T> func)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                return func(fileName);
            }
            finally
            {
                Console.WriteLine($"Time to open {Path.GetFileName(fileName)} is {sw.ElapsedMilliseconds}msec");
            }
        }

        public static void OutputStats(G3D g)
        {
            //Console.WriteLine("Header");

            Console.WriteLine($"# corners per faces {g.NumCornersPerFace} ");
            Console.WriteLine($"# vertices = {g.NumVertices}");
            Console.WriteLine($"# faces = {g.NumFaces}");
            Console.WriteLine($"# subgeos = {g.NumMeshes}");
            Console.WriteLine($"# indices (corners/edges0 = {g.NumCorners}");
            Console.WriteLine($"# instances = {g.NumInstances}");
            Console.WriteLine($"Number of attributes = {g.Attributes.Count}");

            foreach (var attr in g.Attributes.ToEnumerable())
                Console.WriteLine($"{attr.Name} #items={attr.ElementCount}");
        }

        public static void AssertSame(G3D g1, G3D g2)
        {
            Assert.AreEqual(g1.NumCornersPerFace, g2.NumCornersPerFace);
            Assert.AreEqual(g1.NumFaces, g2.NumFaces);
            Assert.AreEqual(g1.NumCorners, g2.NumCorners);
            Assert.AreEqual(g1.NumVertices, g2.NumVertices);
            Assert.AreEqual(g1.NumInstances, g2.NumInstances);
            Assert.AreEqual(g1.NumMeshes, g2.NumMeshes);
            Assert.AreEqual(g1.Attributes.Count, g2.Attributes.Count);
            for (var i = 0; i < g1.Attributes.Count; ++i)
            {
                var attr1 = g1.Attributes[i];
                var attr2 = g2.Attributes[i];
                Assert.AreEqual(attr1.Name, attr2.Name);
                Assert.AreEqual(attr1.GetByteSize(), attr2.GetByteSize());
                Assert.AreEqual(attr1.ElementCount, attr2.ElementCount);
            }
        }

        public static void AssertSame(Mesh m, G3D g)
        {
            Assert.AreEqual(m.FaceCount, g.NumFaces);
            Assert.AreEqual(m.GetIndices(), g.Indices.ToArray());
            Assert.AreEqual(m.VertexCount, g.NumVertices);
        }

        public static G3D CompareTiming(string fileName, string outputFolder)
        {
            using (var context = new AssimpContext())
            {
                var scene = TimeLoadingFile(fileName, context.ImportFile);
                var m = scene.Meshes[0];
                var g3d = m.ToG3D();
                AssertSame(m, g3d);
                var outputFile = Path.Combine(outputFolder, Path.GetFileName(fileName) + ".g3d");
                g3d.Write(outputFile);
                var r = TimeLoadingFile(outputFile, G3D.Read);
                //OutputG3DStats(g3d);
                AssertSame(g3d, r);
                return r;
            }
        }

        public static string[] TestFiles =
        {
            @"models-nonbsd\3DS\jeep1.3ds",
            @"models-nonbsd\3DS\mar_rifle.3ds",
            @"models-nonbsd\dxf\rifle.dxf",
            @"models-nonbsd\FBX\2013_ASCII\duck.fbx",
            @"models-nonbsd\FBX\2013_ASCII\jeep1.fbx",
            // Binary fails assimp import
            //@"models-nonbsd\FBX\2013_BINARY\duck.fbx",
            //@"models-nonbsd\FBX\2013_BINARY\jeep1.fbx",
            // OBJ files were not checked in to the repo.
            //@"models-nonbsd\OBJ\rifle.obj",
            //@"models-nonbsd\OBJ\segment.obj",
            @"models-nonbsd\PLY\ant-half.ply",
            @"models\IFC\AC14-FZK-Haus.ifc",
            @"models\PLY\Wuson.ply",
            @"models\STL\Wuson.stl",
            @"models\STL\Spider_ascii.stl",
            @"models\STL\Spider_binary.stl",
            @"models\glTF\CesiumMilkTruck\CesiumMilkTruck.gltf",
            @"models\glTF2\2CylinderEngine-glTF-Binary\2CylinderEngine.glb",
            @"models\DXF\wuson.dxf",
            @"models\Collada\duck.dae",
        };
    }
}
