using UnityEngine;
using UnityEditor;
using System.Collections;

public class PrimitiveMeshCreator
{
    [MenuItem("Handy Utilities/Primitive Mesh/Create Cube")]
    public static void CreateCube()
    {
        var cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var cubeMesh = cubeObj.GetComponent<MeshFilter>().sharedMesh;
        var mesh = CopyMesh(cubeMesh);
        var path = EditorUtility.SaveFilePanel("Save Mesh", "Assets", "cube", "asset");
        AssetDatabase.CreateAsset(mesh, HandyUtilities.Helper.ConvertLoRelativePath(path));
        Object.DestroyImmediate(cubeObj);
    }

    [MenuItem("CONTEXT/MeshFilter/Copy Scaled Mesh")]
    public static void CopyScaledMesh(MenuCommand command)
    {
        var mf = command.context as MeshFilter;
        var mesh = CopyMesh(mf.sharedMesh, mf.transform.localScale, mf.transform.parent != null ? mf.transform.localPosition : Vector3.zero);
        var path = EditorUtility.SaveFilePanel("Save Mesh", "Assets", "cube", "asset");
        AssetDatabase.CreateAsset(mesh, HandyUtilities.Helper.ConvertLoRelativePath(path));
    }

    [MenuItem("CONTEXT/MeshFilter/Copy And Replace Scaled Mesh")]
    public static void CopyAndReplaceScaledMesh(MenuCommand command)
    {
        var mf = command.context as MeshFilter;
        Undo.RecordObject(mf.gameObject, "Mesh swap");
        var mesh = CopyMesh(mf.sharedMesh, mf.transform.localScale, mf.transform.parent != null ? mf.transform.localPosition : Vector3.zero);
        var path = EditorUtility.SaveFilePanel("Save Mesh", "Assets", "cube", "asset");
     
        if(mf.transform.parent)
        {
            mf.transform.localPosition = Vector3.zero;
        }
        var box = mf.GetComponent<BoxCollider>();
        if(box)
        {
            box.center = mesh.bounds.center;
            box.size = mesh.bounds.size;
        }
        mf.transform.localScale = Vector3.one;
        mf.sharedMesh = mesh;
        EditorUtility.SetDirty(mf.gameObject);  
        AssetDatabase.CreateAsset(mesh, HandyUtilities.Helper.ConvertLoRelativePath(path));
    }


    static Mesh CopyMesh(Mesh original)
    {
        var mesh = new Mesh();
        mesh.vertices = original.vertices;
        mesh.triangles = original.triangles;
        mesh.uv = original.uv;
        mesh.normals = original.normals;
        mesh.tangents = original.tangents;
        mesh.RecalculateBounds();
        return mesh;
    }

    static Mesh CopyMesh(Mesh original, Vector3 scale, Vector3 pivot)
    {
        var mesh = new Mesh();
        var vertices = original.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vector3.Scale(vertices[i], scale) + pivot;
        }
        mesh.vertices = vertices;
        mesh.triangles = original.triangles;
        mesh.uv = original.uv;
        mesh.normals = original.normals;
        mesh.tangents = original.tangents;
        mesh.RecalculateBounds();
        return mesh;
    }

}
