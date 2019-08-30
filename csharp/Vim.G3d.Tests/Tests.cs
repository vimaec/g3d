using NUnit.Framework;
using System;
using System.IO;
using Assimp;

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

        public static string TestOutputFolder => Path.Combine(Path.GetTempPath(), "g3d-test");

        public static int MeshCount = 0;

        public static void TestG3D(G3D g3d, string baseName)
        {
            Console.WriteLine("Testing G3D " + baseName);
            OutputG3DStats(g3d);

            /*
            var buffers = g3d.ToBuffers();
            var i = 0;
            foreach (var buffer in buffers)
            {
                Console.WriteLine(@"Buffer {i++} " + buffer.Name);
            }
            */

            var outputFile = Path.Combine(TestOutputFolder, MeshCount++ + Path.GetFileName(baseName) + ".g3d");
            g3d.Write(outputFile);

            var tmp = G3D.Read(outputFile);
            OutputG3DStats(tmp);

            // TODO: compare tmp and g3d
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

        [Test]
        public static void TestAssimp()
        {
            Directory.CreateDirectory(TestOutputFolder);

            Console.WriteLine(InputDataPath);

            foreach (var f in TestFiles)
            {
                Console.WriteLine("Parsing " + f);
                var file = Path.Combine(InputDataPath, f);
                using (var context = new AssimpContext())
                {
                    var scene = context.ImportFile(file);
                    OutputSceneStats(scene);
                    foreach (var m in scene.Meshes)
                    {
                        OutputMeshStats(m);
                        var g3d = m.ToG3D();
                        TestG3D(g3d, file);
                    }
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
