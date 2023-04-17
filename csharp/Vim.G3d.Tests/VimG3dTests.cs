using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Vim.BFast;
using Vim.G3d.Attributes;
using Vim.Math3d;

using static Vim.G3d.Tests.TestUtils;

namespace Vim.G3d.Tests;

[TestFixture]
public static class VimG3dTests
{
    [Test]
    public static void AttributeDescriptorTest()
    {
        var attributeNames = new VimAttributeCollection().AttributeNames;
        foreach (var name in attributeNames)
        {
            // Test that the attribute descriptor parsing works as intended.
            var parsed = AttributeDescriptor.TryParse(name, out var desc);
            Assert.IsTrue(parsed);
            Assert.AreEqual(name, desc.Name);
        }
    }

    [Test]
    public static void TriangleTest()
    {
        // Serialize a triangle g3d as bytes and read it back.
        var vertices = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1)
        };

        var indices = new[] { 0, 1, 2 };

        //var g3d = new G3DBuilder()
        //    .AddVertices(vertices.ToIArray())
        //    .AddIndices(indices.ToIArray())
        //    .Add(materialIndices.ToIArray().ToFaceMaterialAttribute())
        //    .ToVimG3d();
        var g3d1 = new G3d<VimAttributeCollection>();
        var ac1 = g3d1.AttributeCollection;
        ac1.CornersPerFaceAttribute.TypedData = new[] { 3 };
        ac1.VertexAttribute.TypedData = vertices;
        ac1.IndexAttribute.TypedData = indices;

        var bytes = g3d1.ToBytes();

        if (!G3d<VimAttributeCollection>.TryRead(bytes, out var g))
            Assert.Fail("Could not read g3d bytes.");

        Assert.IsNotNull(g);

        var ac2 = g.AttributeCollection;

        Assert.AreEqual(3, ac2.GetVertexCount());
        Assert.AreEqual(3, ac2.GetIndexCount());
        Assert.AreEqual(3, ac2.GetCornersPerFaceCount());
        Assert.AreEqual(0, ac2.GetMeshCount());
        Assert.AreEqual(0, ac2.GetInstanceCount());

        Assert.AreEqual(vertices, ac2.VertexAttribute.TypedData);
        Assert.AreEqual(indices, ac2.IndexAttribute.TypedData);
    }

    [Test]
    public static void UnexpectedG3dTest()
    {
        var testDir = PrepareTestDir();
        var input = new FileInfo(Path.Combine(TestInputFolder, "G3D", "unexpected.g3d"));

        using var stream = input.OpenRead();

        var result = G3d<VimAttributeCollection>.TryRead(stream, out var g3d);
        Assert.IsTrue(result);

        var cornerAttribute = g3d.AttributeCollection.Attributes[IndexAttribute.AttributeName];
        Assert.IsNotNull(cornerAttribute);
    }

    [Test]
    public static void ReadG3dInVimTest()
    {
        var testDir = PrepareTestDir();
        var inputVim = new FileInfo(Path.Combine(TestInputFolder, "VIM", "Mechanical_Room.r2017.vim"));

        using var stream = inputVim.OpenRead();

        G3d<VimAttributeCollection>? g3d = null;
        stream.ReadBFast<object?>((s, name, size) =>
        {
            if (name == "geometry")
                G3d<VimAttributeCollection>.TryRead(s, out g3d);
            else
                stream.Seek(size, SeekOrigin.Current);

            return null;
        });

        Assert.IsNotNull(g3d);
    }

    public static G3d<VimAttributeCollection> CreateTestG3d()
    {
        var g3d = new G3d<VimAttributeCollection>();
        var attrs = g3d.AttributeCollection;

        attrs.CornersPerFaceAttribute.TypedData = new int[] { 3 };
        attrs.VertexAttribute.TypedData = new Vector3[] { Vector3.Zero, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
        attrs.IndexAttribute.TypedData = new int[] { 0, 1, 2, 0, 3, 2, 1, 3, 2 };
        attrs.SubmeshIndexOffsetAttribute.TypedData = new int[] { 0, 3, 6 };
        attrs.SubmeshMaterialAttribute.TypedData = new int[] { 0 };
        attrs.MeshSubmeshOffsetAttribute.TypedData = new int[] { 0 };
        attrs.InstanceTransformAttribute.TypedData = new Matrix4x4[] { Matrix4x4.Identity };
        attrs.InstanceMeshAttribute.TypedData = new int[] { 0 };
        attrs.InstanceParentAttribute.TypedData = new int[] { -1 };
        attrs.MaterialColorAttribute.TypedData = new Vector4[] { new Vector4(0.25f, 0.5f, 0.75f, 1) };
        attrs.MaterialGlossinessAttribute.TypedData = new float[] { .95f };
        attrs.MaterialSmoothnessAttribute.TypedData = new float[] { .5f };

        g3d.Validate();

        return g3d;
    }

    [Test]
    public static void WriteG3dTest()
    {
        var testDir = PrepareTestDir();

        var g3d = CreateTestG3d();

        var g3dFilePath = Path.Combine(testDir!, "test.g3d");
        g3d.Write(g3dFilePath);

        var readResult = G3d<VimAttributeCollection>.TryRead(g3dFilePath, out var readG3d);
        Assert.IsTrue(readResult);

        // Compare the buffers.
        foreach (var attributeName in readG3d.AttributeCollection.AttributeNames)
        {
            var attr0 = g3d.AttributeCollection.Attributes[attributeName];
            var attr1 = readG3d.AttributeCollection.Attributes[attributeName];
            Assert.AreEqual(attr0.Data, attr1.Data);
        }
    }

    [Test]
    public static void MergeG3dTest()
    {
        var testDir = PrepareTestDir();

        var g3d = CreateTestG3d();

        var mergedG3d = new[] { g3d, g3d }.Merge();

        var g3dFilePath = Path.Combine(testDir!, "merged.g3d");
        mergedG3d.Write(g3dFilePath);

        var readResult = G3d<VimAttributeCollection>.TryRead(g3dFilePath, out var readG3d);
        Assert.IsTrue(readResult);

        var mac = readG3d.AttributeCollection;

        {
            var merged = g3d.AttributeCollection.CornersPerFaceAttribute.TypedData;
            Assert.AreEqual(merged, mac.CornersPerFaceAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.VertexAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.VertexAttribute.TypedData);
        }

        {
            var tmp = g3d.AttributeCollection.IndexAttribute.TypedData;
            var merged = new[] { tmp, tmp.Select(i => i + g3d.AttributeCollection.GetVertexCount()).ToArray() }.SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.IndexAttribute.TypedData);
        }

        {
            var tmp = g3d.AttributeCollection.SubmeshIndexOffsetAttribute.TypedData;
            var merged = new[] { tmp, tmp.Select(i => i + g3d.AttributeCollection.GetIndexCount()).ToArray() }.SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.SubmeshIndexOffsetAttribute.TypedData);
        }

        {
            var tmp = g3d.AttributeCollection.SubmeshMaterialAttribute.TypedData;
            var merged = new[] { tmp, tmp.Select(i => i + g3d.AttributeCollection.GetMaterialCount()).ToArray() }.SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.SubmeshMaterialAttribute.TypedData);
        }

        {
            var tmp = g3d.AttributeCollection.MeshSubmeshOffsetAttribute.TypedData;
            var merged = new[] { tmp, tmp.Select(i => i + g3d.AttributeCollection.GetSubmeshCount()).ToArray() }.SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.MeshSubmeshOffsetAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.InstanceTransformAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.InstanceTransformAttribute.TypedData);
        }

        {
            var tmp = g3d.AttributeCollection.InstanceMeshAttribute.TypedData;
            var merged = new[] { tmp, tmp.Select(i => i + g3d.AttributeCollection.GetMeshCount()).ToArray() }.SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.InstanceMeshAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.InstanceParentAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.InstanceParentAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.MaterialColorAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.MaterialColorAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.MaterialGlossinessAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.MaterialGlossinessAttribute.TypedData);
        }

        {
            var merged = Enumerable.Repeat(g3d.AttributeCollection.MaterialSmoothnessAttribute.TypedData, 2).SelectMany(v => v).ToArray();
            Assert.AreEqual(merged, mac.MaterialSmoothnessAttribute.TypedData);
        }
    }
}