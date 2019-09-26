using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Assimp;
using static Vim.G3d.Tests.Utils;

namespace Vim.G3d.Tests
{
    public static class Tests
    {
        [Test]
        public static void TestPerformance()
        {
            Directory.CreateDirectory(TestOutputFolder);

            Console.WriteLine(InputDataPath);

            foreach (var f in TestFiles)
            {
                var file = Path.Combine(InputDataPath, f);
                CompareTiming(file, TestOutputFolder); 
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
        public static void TestDragon()
        {
            var inputFile = Path.Combine(BaseInputDataPath, "dragon_vrip.ply");
            var g3d = CompareTiming(inputFile, TestOutputFolder);
            OutputG3DStats(g3d);
            var objFile = Path.ChangeExtension(inputFile, "obj");
            g3d.WriteObjFile(objFile);
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

        [Test, Explicit("Big files, hard coded paths")]
        public static void LucyTest()
        {
            var inputFile = @"C:\dev\data\lucy.ply";
            var outputFolder = @"C:\dev\data\output";
            var g3d = CompareTiming(inputFile, outputFolder);
            g3d.WriteObjFile(Path.Combine(outputFolder, Path.GetFileName(inputFile) + ".obj"));
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
            Assert.AreEqual(7, g3d.Attributes.Count);
            Assert.AreEqual(new[]
            {
                CommonAttributes.ObjectFaceSize,
                CommonAttributes.Position,
                CommonAttributes.Indices,
                CommonAttributes.UV,
                CommonAttributes.VertexColor,
                CommonAttributes.GroupIndices,
                CommonAttributes.MaterialIds
            }, g3d.Attributes.Select(attr => attr.Descriptor.ToString()).ToArray());
        }
    }
}
