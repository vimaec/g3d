using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Assimp;
using Assimp.Configs;

namespace Vim.G3d.Tests
{
    public static class Tests
    {
        public static void OutputSceneStats(Scene scene)
        {
            Console.WriteLine(
$@"       #animations = {scene.AnimationCount}
    #cameras = {scene.CameraCount}
    #lights = {scene.LightCount}
    #mterials = {scene.MaterialCount}
    #meshes = {scene.MeshCount}
    #textures = {scene.TextureCount}");
        }

        // TODO: merge all of the meshes using the transform. 

        public static void OutputMeshStats(Mesh mesh)
        {
            Console.WriteLine(
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
                    }

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

        public static void OutputG3DStats(G3D g)
        {
            Console.WriteLine($"Number of attributes = {g.Attributes.Count}");
            //Console.WriteLine("Header");
            foreach (var attr in g.Attributes)
            {
                Console.WriteLine($"{attr.Name} #bytes={attr.Bytes.Length} #items={attr.ElementCount}");
            }
            Console.WriteLine($"{g.CornersPerFace} corners per face");
        }

        public static string InputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "..", // yes 5, count em, 5  
            "data", "assimp", "test");

        public static string TestOutputFolder => Path.Combine(InputDataPath, "..", "..", "g3d");

        public static void TestG3D(G3D g3d, string baseName)
        {
            Console.WriteLine("Testing G3D " + baseName);
            OutputG3DStats(g3d);

            var outputFile = Path.Combine(TestOutputFolder, Path.GetFileName(baseName) + ".g3d");
            g3d.Write(outputFile);

            var tmp = TimeLoadingFile(outputFile, G3D.Read);
            OutputG3DStats(tmp);
        }

        public static void CompareTiming(string fileName)
        {
            using (var context = new AssimpContext())
            {
                var scene = TimeLoadingFile(fileName, context.ImportFile);
                var m = scene.Meshes[0];
                var g3d = m.ToG3D();
                var outputFile = Path.Combine(TestOutputFolder, Path.GetFileName(fileName) + ".g3d");
                g3d.Write(outputFile);
                TimeLoadingFile(outputFile, G3D.Read);
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
            @"models-nonbsd\OBJ\rifle.obj",
            @"models-nonbsd\OBJ\segment.obj",
            @"models-nonbsd\PLY\ant-half.ply",
            @"models\IFC\AC14-FZK-Haus.ifc",
            @"models\PLY\wuson.ply",
            @"models\STL\wuson.stl",
            @"models\STL\Spider_ascii.stl",
            @"models\STL\Spider_binary.stl",
            @"models\glTF\CesiumMilkTruck\CesiumMilkTruck.gltf",
            @"models\glTF2\2CylinderEngine-glTF-Binary\2CylinderEngine.glb",
            @"models\DXF\wuson.dxf",
            @"models\Collada\duck.dae",
        };

        [Test, Explicit("Performance")]
        public static void TestPerformance()
        {
            Directory.CreateDirectory(TestOutputFolder);

            Console.WriteLine(InputDataPath);

            foreach (var f in TestFiles)
            {
                var file = Path.Combine(InputDataPath, f);
                CompareTiming(file); 
            }
        }

        [Test]
        public static void TestAssimp()
        {
            Directory.CreateDirectory(TestOutputFolder);

            Console.WriteLine(InputDataPath);

            foreach (var f in TestFiles)
            {
                var file = Path.Combine(InputDataPath, f);
                using (var context = new AssimpContext())
                {
                    var scene = TimeLoadingFile(file, context.ImportFile);
                    OutputSceneStats(scene);

                    // We only grab the first mesh. 
                    var m = scene.Meshes[0];
                    OutputMeshStats(m);
                    var g3d = m.ToG3D();
                    TestG3D(g3d, file);
                }
            }
        }

        public static G3D LoadAssimpFile(string filePath)
        {
           using (var context = new AssimpContext())
           {
                var scene = context.ImportFile(filePath);
                Assert.AreEqual(1, scene.Meshes.Count);
                return scene.Meshes[0].ToG3D();
           }
        }

        public static void CompareG3D(G3D g1, G3D g2, bool compareAllAttributes = false)
        {
            if (compareAllAttributes)
            {
                Assert.AreEqual(g1.Attributes.Count, g2.Attributes.Count);
                for (var i = 0; i < g1.Attributes.Count; ++i)
                    Assert.AreEqual(g1.Attributes[i].Descriptor.ToString(), g2.Attributes[i].Descriptor.ToString());
            }

            Assert.AreEqual(g1.CornersPerFace, g2.CornersPerFace);
            Assert.AreEqual(g1.NumCorners, g2.NumCorners);
            Assert.AreEqual(g1.NumFaces, g2.NumFaces);
            Assert.AreEqual(g1.NumGroups, g2.NumGroups);
            Assert.AreEqual(g1.NumVertices, g2.NumVertices);

            Assert.AreEqual(g1.Indices.Data.ToArray(), g2.Indices.Data.ToArray());

            for (var i=0; i < g1.NumVertices; ++i)
                Assert.AreEqual(g1.Vertices.Data[i], g2.Vertices.Data[i], 0.001);
        }

        [Test]
        public static void TestWriters()
        {
            var fileName = @"models\PLY\wuson.ply";
            fileName = Path.Combine(InputDataPath, fileName);

            var outputFileName = @"test";
            outputFileName = Path.Combine(TestOutputFolder, outputFileName);

            var g3d = LoadAssimpFile(fileName);

            g3d.WritePlyFile(outputFileName + ".ply");
            g3d.WriteObjFile(outputFileName + ".obj");

            // TODO compare the PLY, the OBJ and the original file.

            var g3dFromPly = LoadAssimpFile(outputFileName + ".ply");
            var g3dFromObj = LoadAssimpFile(outputFileName + ".obj");

            CompareG3D(g3d, g3dFromPly);

            // BUG: Assimp ignores the OBJ index buffer. God knows why. 
            //CompareG3D(g3d, g3dFromObj);
        }

        [Test]
        public static void TestBuilder()
        {
            var gb = new G3DBuilder();

            var vertices = new[] {0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f, 1f, 1f, 1f, };
            var colors = new[] { 1f, 1f, 1f, 0f, 0f, 0f, 1f, 1f, 1f, 0f, 0f, 0f, };
            var uvs = new[] { 0f, 0f, 0.2f, 0.2f, 0.5f, 0.5f, 0.8f, 0.8f };
            var indices = new[] {0, 1, 2, 0, 1, 3, 1, 2, 3, 2, 3, 0 };
            var materialIds = new[] {0, 1, 2, 1};
            gb.SetObjectFaceSize(3);
            gb.AddVertices(vertices);
            gb.AddIndices(indices);
            gb.AddUV(uvs);
            gb.AddVertexColors(colors);
            gb.AddGroupIndexOffsets(new[] {0, 6});
            gb.AddMaterialIds(materialIds);
            var g3d = gb.ToG3D();
            Assert.AreEqual(4, g3d.NumVertices);
            Assert.AreEqual(3, g3d.CornersPerFace);
            Assert.AreEqual(4, g3d.NumFaces);
            Assert.AreEqual(2, g3d.NumGroups);
            Assert.AreEqual(vertices, g3d.Vertices.Data.ToArray());
            Assert.AreEqual(colors, g3d.VertexColor[0].Data.ToArray());
            Assert.AreEqual(uvs, g3d.UV[0].Data.ToArray());
            Assert.AreEqual(2, g3d.Groups.Length);
        }
    }
}
