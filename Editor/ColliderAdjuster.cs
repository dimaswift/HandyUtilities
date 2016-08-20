using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HandyUtilities
{

    public class ColliderAdjuster : Editor
    {
        [MenuItem("CONTEXT/BoxCollider2D/Adjust")]
        public static void AdjustBoxCollider2D(MenuCommand command)
        {

            var box = command.context as BoxCollider2D;
            Undo.RecordObject(box, "Adjust Collider2D");
            var mesh = box.GetComponentInChildren<MeshFilter>();
            if (mesh)
            {
                var size = Vector3.Scale(mesh.sharedMesh.bounds.size, mesh.transform.localScale);
                box.size = size;
                box.offset = mesh.transform.localPosition;
                EditorUtility.SetDirty(box);
            }

        }

        [MenuItem("CONTEXT/PolygonCollider2D/Adjust")]
        public static void AdjustPolygonCollider2D(MenuCommand command)
        {

            var poly = command.context as PolygonCollider2D;
            Undo.RecordObject(poly, "Adjust Collider2D");
            var mesh = poly.GetComponentInChildren<MeshFilter>();
            if (mesh)
            {
                //   var size = Vector3.Scale(mesh.sharedMesh.bounds.size, mesh.transform.localScale);
                var points = new Vector2[4];
                var size = Vector3.Scale(mesh.sharedMesh.bounds.extents, mesh.transform.localScale);
                points[0] = poly.transform.InverseTransformPoint(RotatePointAroundPivot((mesh.transform.position + new Vector3(size.x, size.y)), mesh.transform.position, mesh.transform.localEulerAngles));
                points[1] = poly.transform.InverseTransformPoint(RotatePointAroundPivot((mesh.transform.position + new Vector3(-size.x, size.y)), mesh.transform.position, mesh.transform.localEulerAngles));
                points[2] = poly.transform.InverseTransformPoint(RotatePointAroundPivot((mesh.transform.position + new Vector3(-size.x, -size.y)), mesh.transform.position, mesh.transform.localEulerAngles));
                points[3] = poly.transform.InverseTransformPoint(RotatePointAroundPivot((mesh.transform.position + new Vector3(size.x, -size.y)), mesh.transform.position, mesh.transform.localEulerAngles));
                poly.SetPath(0, points);
                EditorUtility.SetDirty(poly);
            }

        }

        [MenuItem("CONTEXT/BoxCollider/Adjust")]
        public static void AdjustBoxCollider(MenuCommand command)
        {

            var box = command.context as BoxCollider;
            Undo.RecordObject(box, "Adjust Collider");
            var mesh = box.transform.GetChild(0).GetComponent<MeshFilter>();
            if (mesh)
            {
                //   var size = Vector3.Scale(mesh.sharedMesh.bounds.size, mesh.transform.localScale);
                var size = Vector3.Scale(mesh.sharedMesh.bounds.size, mesh.transform.localScale);
                box.size = size;
                box.center = mesh.transform.localPosition;
                EditorUtility.SetDirty(box);
            }

        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
    }


}
