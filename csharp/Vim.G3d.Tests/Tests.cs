using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Assimp;
using g3;

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
                Console.WriteLine($"{attr.Name} #bytes={attr.Bytes.Length} #items={attr.Count}");
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

        [Test]
        public static void TestPlyWriter()
        {
            var fileName = @"models\PLY\wuson.ply";
            fileName = Path.Combine(InputDataPath, fileName);

            var outputFileName = @"wuson.ply";
            outputFileName = Path.Combine(TestOutputFolder, outputFileName);

            using (var context = new AssimpContext())
            {
                var scene = context.ImportFile(fileName);

                if (scene.Meshes.Count != 1)
                {
                    throw new Exception("Expected 1 mesh in file.");
                }

                var g3d = scene.Meshes[0].ToG3D();
                g3d.WritePly(outputFileName);
            }
        }
    }
}
