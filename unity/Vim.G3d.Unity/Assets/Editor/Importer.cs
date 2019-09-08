using UnityEditor.Experimental.AssetImporters;
using System.IO;
using UnityEditor;
using UnityEngine;
using Vim.G3d;

public static class ImportHelpers
{
    public static GameObject ImportG3D(this AssetImportContext ctx, G3D g3d, string baseName)
    {
        var obj = ObjectFactory.CreateGameObject(baseName, typeof(MeshFilter), typeof(MeshRenderer));

        // Create a Mesh filter and assign the mesh to it
        var mf = obj.GetComponent<MeshFilter>();
        mf.sharedMesh= new Mesh();
        g3d.CopyTo(mf.sharedMesh);
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
}

[ScriptedImporter(1, "g3d")]
public class G3DImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var baseName = Path.GetFileNameWithoutExtension(ctx.assetPath);
        var g3d = G3D.Read(ctx.assetPath);
        var obj = ctx.ImportG3D(g3d, baseName);
        ctx.AddRandomMaterial(obj);
        ctx.SetMainObject(obj);
    }
}