using Assimp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vim.G3d.AssimpWrapper;
using Vim.G3d.Attributes;
using Vim.Math3d;

using static Vim.G3d.Tests.TestUtils;

using VimG3d = Vim.G3d.G3d<Vim.G3d.Attributes.VimAttributeCollection>;

namespace Vim.G3d.Tests;

[TestFixture]
public static class AssimpTests
{
    public class FileLoadData
    {
        public FileLoadData(string filePath)
            => SourceFile = new FileInfo(filePath);

        public string ShortName => Path.GetFileName(SourceFile.FullName);
        public int NumMeshes => Scene?.MeshCount ?? 0;
        public FileInfo SourceFile;
        public FileInfo G3DFile;
        public long MSecToOpen;
        public long MSecToSaveG3d;
        public long MSecToOpenG3d;
        public long MSecToConvert;
        public long MemoryConsumption;
        public long MemoryConsumptionG3d;
        public Exception Error;
        public Scene Scene;
        public VimG3d G3d;
    }

    public static readonly string ProjectFolder = new DirectoryInfo(Properties.Resources.ProjDir.Trim()).FullName;
    public static string RootFolder = Path.Combine(ProjectFolder, "..", "..");
    public static string TestInputFolder = Path.Combine(RootFolder, "data", "models");
    public static string TestOutputFolder = Path.Combine(RootFolder, "out");

    public static IEnumerable<string> GetInputFiles()
        => Directory.GetFiles(TestInputFolder, "*.*", SearchOption.AllDirectories);

    public static void ValidateSame(Object a, Object b, string name = "")
    {
        if (!a.Equals(b))
            throw new Exception($"Values {a} and {b} are different {name}");
    }

    public static void ValidateSameG3D(VimG3d g1, VimG3d g2)
    {
        var (ac1, ac2) = (g1.AttributeCollection, g2.AttributeCollection);

        ValidateSame(ac1.GetCornersPerFaceCount(), ac2.GetCornersPerFaceCount(), "GetCornersPerFaceCount");
        ValidateSame(ac1.GetIndexCount(), ac2.GetIndexCount(), "GetIndexCount");
        ValidateSame(ac1.GetVertexCount(), ac2.GetVertexCount(), "GetVertexCount");
        ValidateSame(ac1.GetInstanceCount(), ac2.GetInstanceCount(), "GetInstanceCount");
        ValidateSame(ac1.GetMeshCount(), ac2.GetMeshCount(), "GetMeshCount");
        ValidateSame(ac1.Attributes.Count, ac2.Attributes.Count, "Attribute Count");

        foreach (var (k, attr1) in ac1.Attributes)
        {
            var attr2 = ac2.Attributes[k];
            ValidateSame(attr1.Name, attr2.Name, $"Attribute[{k}].Name");
            ValidateSame(attr1.Data.Length, attr2.Data.Length, $"Attribute[{k}].Data.Length");
        }
    }

    [Test]
    [Platform(Exclude = "Linux,Unix", Reason = "AssimpNet is failing to load its dependency on 'libdl.so'.")]
    public static void OpenAndConvertAssimpFiles()
    {
        var testDir = PrepareTestDir();

        var files = GetInputFiles()
            .Where(AssimpLoader.CanLoad)
            .Select(f => new FileLoadData(f))
            .ToArray();

        // Load all the files 
        foreach (var f in files)
        {
            try
            {
                (f.MemoryConsumption, f.MSecToOpen) =
                    TestUtils.GetMemoryConsumptionAndMSecElapsed(() =>
                        f.Scene = AssimpLoader.Load(f.SourceFile.FullName));
            }
            catch (Exception e)
            {
                f.Error = e;
            }
        }

        // Convert all the Assimp scenes to G3D
        foreach (var f in files)
        {
            if (f.Scene == null) continue;

            try
            {
                f.MSecToConvert = TestUtils.GetMSecElapsed(() =>
                    f.G3d = f.Scene.ToG3d());
            }
            catch (Exception e)
            {
                f.Error = e;
            }
        }

        // Save all the G3D scenes
        foreach (var f in files)
        {
            if (f.G3d == null) continue;

            try
            {
                var outputFilePath = Path.Combine(testDir, f.ShortName + ".g3d");
                f.G3DFile = new FileInfo(outputFilePath);

                f.MSecToSaveG3d = TestUtils.GetMSecElapsed(() =>
                    f.G3d.Write(outputFilePath));
            }
            catch (Exception e)
            {
                f.Error = e;
            }
        }

        // Try reading back in all of the G3D scenes, measure load times and the memory consumption
        foreach (var f in files)
        {
            if (f.G3DFile == null) continue;

            try
            {
                VimG3d localG3d = null;

                (f.MemoryConsumptionG3d, f.MSecToOpenG3d) =
                    TestUtils.GetMemoryConsumptionAndMSecElapsed(() =>
                        VimG3d.TryRead(f.G3DFile.FullName, out localG3d));

                ValidateSameG3D(f.G3d, localG3d);
            }
            catch (Exception e)
            {
                f.Error = e;
            }
        }

        // Output the header for data
        Console.WriteLine(
            "Importer," +
            "Extension," +
            "File Name," +
            "File Size(KB)," +
            "Load Time(s)," +
            "Memory(KB)," +
            "# Meshes," +
            "Time to Convert," +
            "Time to Write G3D," +
            "G3D File Size(KB)," +
            "G3D Memory(KB)",
            "G3D Load Time(s)",
            "Error");

        // Output the data rows
        foreach (var f in files)
        {
            Console.WriteLine(
                "Assimp," +
                $"{Path.GetExtension(f.ShortName)}," +
                $"{f.ShortName}," +
                $"{f.SourceFile?.Length / 1000}," +
                $"{f.MSecToOpen / 100f}," +
                $"{f.MemoryConsumption / 1000}," +
                $"{f.NumMeshes}," +
                $"{f.MSecToConvert / 100f}," +
                $"{f.MSecToSaveG3d / 100f}," +
                $"{f.G3DFile?.Length / 1000}," +
                $"{f.MemoryConsumptionG3d / 1000}," +
                $"{f.MSecToOpenG3d / 100f}," +
                $"{f.Error}");
        }

        Assert.AreEqual(0, files.Count(f => f.Error != null), "Errors occurred");
    }

    public static VimG3d LoadAssimpFile(string filePath)
    {
        using (var context = new AssimpContext())
        {
            var scene = context.ImportFile(filePath);
            Assert.AreEqual(1, scene.Meshes.Count);
            return scene.Meshes[0].ToVimG3d();
        }
    }

    [Test]
    [Platform(Exclude = "Linux,Unix", Reason = "AssimpNet is failing to load its dependency on 'libdl.so'.")]
    public static void TestWriters()
    {
        var testDir = PrepareTestDir();

        var fileName = Path.Combine(TestInputFolder, "PLY", "Wuson.ply");

        var outputFileName = @"test";
        outputFileName = Path.Combine(testDir, outputFileName);

        var g3d = LoadAssimpFile(fileName);

        g3d.WritePly(outputFileName + ".ply");
        g3d.WriteObj(outputFileName + ".obj");

        // TODO compare the PLY, the OBJ and the original file.

        var g3dFromPly = LoadAssimpFile(outputFileName + ".ply");
        var g3dFromObj = LoadAssimpFile(outputFileName + ".obj");

        {
            var ac1 = g3d.AttributeCollection;
            var ac2 = g3dFromPly.AttributeCollection;
            Assert.AreEqual(ac1.GetCornersPerFaceCount(), ac2.GetCornersPerFaceCount());
            Assert.AreEqual(ac1.GetFaceCount(), ac2.GetFaceCount());
            Assert.AreEqual(ac1.GetIndexCount(), ac2.GetIndexCount());
            Assert.AreEqual(ac1.GetVertexCount(), ac2.GetVertexCount());
            Assert.AreEqual(ac1.GetInstanceCount(), ac2.GetInstanceCount());
            Assert.AreEqual(ac1.GetMeshCount(), ac2.GetMeshCount());
        }

        // BUG: Assimp ignores the OBJ index buffer. God knows why. 
        //CompareG3D(g3d, g3dFromObj);
    }
}