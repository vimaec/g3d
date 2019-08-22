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
            foreach (var attr in g.Attributes)
            {
                Console.WriteLine($"{attr.Name} #bytes={attr.Bytes.Length} #items={attr.Count}");
            }
        }

        public static string InputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "..", // yes 5, count em, 5  
            "data", "assimp", "test");

        [Test]
        public static void TestAssimp()
        {
            Console.WriteLine(InputDataPath);
            var file = Path.Combine(InputDataPath, "models", "STL", "wuson.stl");
            using (var context = new AssimpContext())
            {
                var scene = context.ImportFile(file);
                OutputSceneStats(scene);
                foreach (var m in scene.Meshes)
                {
                    OutputMeshStats(m);
                    var g3d = m.ToG3D();
                    OutputG3DStats(g3d);
                }
            }
        }
    }
}
