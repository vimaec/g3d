using System;
using System.Diagnostics;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using UnityEditor;
using UnityEngine;
using Vim.G3d;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public static class ImportHelpers
{
    public static void TimeBlockingOperation()
    {
        var sw = new Stopwatch();
        sw.Start();
        EditorApplication.delayCall += () => {
            sw.Stop();
            Debug.Log("Operation took: " + sw.Elapsed);
        };
    }

    public static void TimeOperation(Action action, string message = "")
    {
        Debug.Log("Starting timed operation " + message);
        var sw = new Stopwatch();
        sw.Start();
        action();
        sw.Stop(); Debug.Log("Operation took: " + sw.Elapsed);
    }

    public static GameObject ImportG3D(this AssetImportContext ctx, G3D g3d, string baseName)
    {
        var obj = ObjectFactory.CreateGameObject(baseName, typeof(MeshFilter), typeof(MeshRenderer));

        // Create a Mesh filter and assign the mesh to it
        var mf = obj.GetComponent<MeshFilter>();
        mf.sharedMesh = g3d.ToMesh();
        var nFace = g3d.NumFaces;
        var nVert = g3d.Vertices.ElementCount;
        var nAttr = g3d.Attributes.Count;
        var meshName = $"mesh {baseName} ({nFace} polys, {nVert} verts, {nAttr} attributes)";
        ctx.AddObjectToAsset(meshName, mf.sharedMesh);

        // Add the game object as an asset
        ctx.AddObjectToAsset(baseName, obj);
        return obj;
    }

    public static GameObject AddRandomMaterial(this AssetImportContext ctx, GameObject obj)
    {
        // Create a random material and material renderer
        var mr = obj.GetComponent<MeshRenderer>();
        var material = new Material(Shader.Find("Standard"));
        mr.material = material;
        material.color = Random.ColorHSV(0, 1, 0.1f, 0.9f, 0.1f, 0.9f);
        ctx.AddObjectToAsset($"material ({material.color})", material);
        return obj;
    }

    public static void OutputStats(this G3D g)
    {
        Debug.Log($"# corners per faces {g.CornersPerFace} ");
        Debug.Log($"# vertices = {g.NumVertices}");
        Debug.Log($"# faces = {g.NumFaces}");
        Debug.Log($"# groups = {g.NumGroups}");
        Debug.Log($"Number of attributes = {g.Attributes.Count}");
        //Debug.Log("Header");
        foreach (var attr in g.Attributes)
            Debug.Log($"{attr.Name} #bytes={attr.Bytes.Length} #items={attr.ElementCount}");
    }

    public static Mesh GetMesh(this GameObject obj)
    {
        if (obj == null) return null;
        var mf = obj.GetComponent<MeshFilter>();
        if (mf == null) return null;
        var mesh = mf.sharedMesh;
        if (mesh == null)
            mesh = mf.mesh;
        return mesh;
    }
}

[ScriptedImporter(1, "g3d")]
public class G3DImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        Debug.Log($"Importing G3D file: {ctx.assetPath}");
        ImportHelpers.TimeBlockingOperation();
        var baseName = Path.GetFileNameWithoutExtension(ctx.assetPath);
        var g3d = G3D.Read(ctx.assetPath);
        g3d.OutputStats();
        var obj = ctx.ImportG3D(g3d, baseName);
        ctx.AddRandomMaterial(obj);
        ctx.SetMainObject(obj);
    }
}

public class G3DExporter : MonoBehaviour
{
    [MenuItem("VIM/Export G3D")]
    static void Apply()
    {
        var obj = Selection.activeObject as GameObject;
        var mesh = obj != null ? obj.GetMesh() : null;
        if (mesh == null)
        {
            EditorUtility.DisplayDialog("Export G3D", "You must select a mesh first", "OK");
            return;
        }

        var name = string.IsNullOrWhiteSpace(obj.name) ? "mesh" : obj.name;
        var path = EditorUtility.SaveFilePanel("Export G3D", "", name, "g3d");

        if (path.Length != 0)
        {
            mesh.ToG3D().Write(path);
            Debug.Log("Saved mesh to " + path);
        }
    }
}

public class MeshStats : MonoBehaviour
{
    [MenuItem("VIM/Mesh Stats")]
    static void Apply()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj == null) return;
        var mesh = obj.GetMesh();
        if (mesh == null) return;
        Debug.Log($"# vertices = {mesh.vertexCount}");
        Debug.Log($"# triangles = {mesh.triangles.Length}");
        Debug.Log($"# sub-meshes = {mesh.subMeshCount}");
        Debug.Log($"# normals = {mesh.normals?.Length ?? 0}");
        Debug.Log($"# UVs = {mesh.uv?.Length ?? 0}");
        Debug.Log($"# colors = {mesh.colors?.Length ?? 0}");
    }
}