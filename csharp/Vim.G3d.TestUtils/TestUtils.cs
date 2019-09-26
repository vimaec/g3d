using Assimp;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Vim.G3d.Tests
{
    public class Utils
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
            Console.WriteLine($"# corners per faces {g.CornersPerFace} ");
            Console.WriteLine($"# vertices = {g.NumVertices}");
            Console.WriteLine($"# faces = {g.NumFaces}");
            Console.WriteLine($"# groups = {g.NumGroups}");
            Console.WriteLine($"Number of attributes = {g.Attributes.Count}");
            //Console.WriteLine("Header");
            foreach (var attr in g.Attributes)
            {
                Console.WriteLine($"{attr.Name} #bytes={attr.Bytes.Length} #items={attr.ElementCount}");
            }
        }
    
        // This depends on where the executable ends up (which is different between .NET standard and .NET framework and .NET core)
        public static string BaseInputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "..", "data"); // yes 5, count em, 5  

        public static string InputDataPath => Path.Combine(BaseInputDataPath, "assimp", "test");

        public static string TestOutputFolder => Path.Combine(InputDataPath, "..", "..", "g3d");

        public static string[] GeneratedG3ds => Directory.GetFiles(TestOutputFolder, "*.g3d");

        public static void TestG3D(G3D g3d, string baseName)
        {
            Console.WriteLine("Testing G3D " + baseName);
            OutputG3DStats(g3d);

            var outputFile = Path.Combine(TestOutputFolder, Path.GetFileName(baseName) + ".g3d");
            g3d.Write(outputFile);

            var tmp = TimeLoadingFile(outputFile, G3D.Read);
            OutputG3DStats(tmp);
        }

        public static G3D CompareTiming(string fileName, string outputFolder)
        {
            using (var context = new AssimpContext())
            {
                var scene = TimeLoadingFile(fileName, context.ImportFile);
                var m = scene.Meshes[0];
                var g3d = m.ToG3D();
                var outputFile = Path.Combine(outputFolder, Path.GetFileName(fileName) + ".g3d");
                g3d.Write(outputFile);
                TimeLoadingFile(outputFile, G3D.Read);
                OutputG3DStats(g3d);
                return g3d;
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
    }
}
