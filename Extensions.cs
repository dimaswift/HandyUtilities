using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace HandyUtilities
{
    public static class ExtensionMethods
    {
        #region Strings

        public static string ToUpperFirst(this string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1);
        }

        public static string ToLowerFirst(this string s)
        {
            return s[0].ToString().ToLower() + s.Substring(1);
        }

        #endregion Strings

        #region Transform

        /// <summary>
        /// Sets local Z postion of transform.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="z"> Z position. </param> 
        public static void SetLocalZ(this Transform trans, float z)
        {
            var pos = trans.localPosition;
            pos.z = z;
            trans.localPosition = pos;
        }

        public static void RotateAroundPivot(this Transform transform, Vector3 pivot, Vector3 angles)
        {
            var point = transform.position;
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angles) * dir;
            point = dir + pivot;
            transform.localPosition = point;
            transform.LookAt(pivot, Vector3.up);
        }

        public static Transform[] GetChildren(this Transform t)
        {
            var children = new Transform[t.childCount];
            for (int i = 0; i < t.childCount; i++)
            {
                children[i] = t.GetChild(i);
            }
            return children;
        }
        /// <summary>
        /// Sets world Z position of transform.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="z">Z position.</param>
        public static void SetWorldZ(this Transform trans, float z)
        {
            var pos = trans.position;
            pos.z = z;
            trans.position = pos;
        }
        /// <summary>
        /// Rotates transform on Z axis to face the target vector.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="target">Position to face.</param>
        /// <param name="space">Switches between local and world spaces.</param>
        /// <param name="offset">Rotation offset in angles.</param>
        public static void LookAtEuler(this Transform t, Vector3 target, Space space = Space.World, float offset = 0)
        {
            var origin = t.position;
            origin.x = origin.x - target.x;
            origin.y = origin.y - target.y;
            var angle = Mathf.Atan2(origin.y, origin.x) * Mathf.Rad2Deg;
            if (space == Space.World)
            {
                var euler = t.eulerAngles;
                euler.z = angle + offset;
                t.eulerAngles = euler;
            }
            else
            {
                var euler = t.localEulerAngles;
                euler.z = angle + offset;
                t.localEulerAngles = euler;
            }
        }
        /// <summary>
        /// Rotates transform on Z axis to face the target vector.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="target">Position to face.</param>
        /// <param name="origin">Original point that forms angle between target.</param>
        /// <param name="space">Switches between local and world spaces.</param>
        /// <param name="offset">Rotation offset in angles.</param>
        public static void LookAtEuler(this Transform t, Vector3 target, Vector3 origin, float offset = 0, Space space = Space.World)
        {
            origin.x = origin.x - target.x;
            origin.y = origin.y - target.y;
            var angle = Mathf.Atan2(origin.y, origin.x) * Mathf.Rad2Deg;
            if (space == Space.World)
            {
                var euler = t.eulerAngles;
                euler.z = angle + offset;
                t.eulerAngles = euler;
            }
            else
            {
                var euler = t.localEulerAngles;
                euler.z = angle + offset;
                t.localEulerAngles = euler;
            }
        }
        /// <summary>
        /// Rotates transform on Z axis to face the target vector.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetPosition"></param>
        /// <param name="offset"></param>
        public static void MoveWithOffset(this Transform target, Vector3 targetPosition, Vector3 offset)
        {
            targetPosition.x = targetPosition.x + (target.position.x - offset.x);
            targetPosition.y = targetPosition.y + (target.position.y - offset.y);
            targetPosition.z = target.position.z;
            target.position = targetPosition;
        }
        public static void MoveWithSmoothOffset(this Transform target, Vector3 targetPosition, Vector3 offset, float speed)
        {
            targetPosition.x = targetPosition.x + (target.position.x - offset.x);
            targetPosition.y = targetPosition.y + (target.position.y - offset.y);
            targetPosition.z = target.position.z;
            target.position = Vector3.MoveTowards(target.position, targetPosition, speed);
        }

        public static string GetPath(this Transform t, Transform root)
        {
            var parent = t.parent;
            string result = "";
            while (parent && t != root)
            {
                result = "/" + result.Insert(0, t.name);
                t = parent;
                parent = parent.parent;
            }
            if (result.StartsWith("/"))
                result = result.Remove(0, 1);
            return result;
        }
        public static void SetScale(this Transform t, float scale)
        {
            t.localScale = new Vector3(scale, scale, scale);
        }
        public static void ScaleTowards(this Transform t, Vector3 scale, float speed)
        {
            if (!t.IsCloseToScale(scale))
            {
                t.localScale = Vector3.MoveTowards(t.localScale, scale, speed * Time.deltaTime * 100);
            }
        }
        public static void MoveTowardsLocal(this Transform t, Vector3 pos, float speed)
        {
            if (!t.IsCloseToLocalPosition(pos))
            {
                t.localPosition = Vector3.MoveTowards(t.localPosition, pos, speed * Time.deltaTime * 100);
            }
        }
        public static void MoveTowards(this Transform t, Vector3 pos, float speed)
        {
            if (!t.IsCloseToPosition(pos))
            {
                t.position = Vector3.MoveTowards(t.position, pos, speed * Time.deltaTime * 100);
            }
        }
        public static void SmoothLocalMove(this Transform t, Vector3 pos, float speed)
        {
            var zero = Vector3.zero;
            if (!t.IsCloseToLocalPosition(pos))
            {
                t.localPosition = Vector3.SmoothDamp(t.localPosition, pos, ref zero, speed);
            }
        }
        public static void SmoothMove(this Transform t, Vector3 pos, float speed)
        {
            var zero = Vector3.zero;
            if (!t.IsCloseToPosition(pos))
            {
                t.position = Vector3.SmoothDamp(t.position, pos, ref zero, speed);
            }
        }
        public static void SmoothScale(this Transform t, Vector3 scale, float speed)
        {
            var zero = Vector3.zero;
            if (!t.IsCloseToScale(scale))
            {
                t.localScale = Vector3.SmoothDamp(t.localScale, scale, ref zero, speed);
            }
        }
        public static bool IsCloseToScale(this Transform t, Vector3 scale)
        {
            return Vector3.Distance(t.localScale, scale) < float.Epsilon;
        }
        public static bool IsCloseToPosition(this Transform t, Vector3 position)
        {
            return Vector2.Distance(t.position, position) < float.Epsilon;
        }
        public static bool IsCloseToLocalPosition(this Transform t, Vector3 position)
        {
            return Vector2.Distance(t.localPosition, position) < float.Epsilon;
        }
        public static bool IsCloseToPosition(this Transform t, Vector2 position)
        {
            return Vector2.Distance(t.position, position) < float.Epsilon;
        }
        public static void MakeTextureReadable(Texture2D texture)
        {
            if (texture != null)
            {
                //var importer = D2D_Helper.GetAssetImporter<UnityEditor.TextureImporter>(texture);

                //if (importer != null && importer.isReadable == false)
                //{
                //    importer.isReadable = true;

                //    D2D_Helper.ReimportAsset(importer.assetPath);
                //}
            }
        }
        public static void Flip(this Transform t, bool rotateY = false)
        {
            if (t.parent != null)
            {
                var pos = Vector3.Reflect(t.localPosition, Vector2.left);
                pos.z = t.localPosition.z;
                t.localPosition = pos;
            }
            var euler = t.localEulerAngles;
            if (rotateY)
            {
                euler.y = euler.y > 0 ? 0 : 180;
            }
            euler.z = Helper.To180Angle(euler.z * -1);
            t.localEulerAngles = euler;
        }
        public static void Flip(this Transform t, Vector2 direction, bool rotateY = false)
        {
            var pos = Vector3.Reflect(t.localPosition, direction);
            pos.z = t.localPosition.z;
            t.localPosition = pos;
            var euler = t.localEulerAngles;
            if (rotateY)
            {
                euler.y = euler.y > 0 ? 0 : 180;
            }
            euler.z = Helper.To180Angle(euler.z * -1);
            t.localEulerAngles = euler;
        }

        public static void SetLocalEulerZ(this Transform t, float euler)
        {
            var e = t.localEulerAngles;
            e.z = euler;
            t.localEulerAngles = e;
        }
        public static void SetEulerZ(this Transform t, float euler)
        {
            var e = t.eulerAngles;
            e.z = euler;
            t.eulerAngles = e;
        }
        public static void ResetAll(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
        }
        #endregion

        #region Hinge Joint2D

        /// <summary>
        /// Joins connected anchor and anchor in one point
        /// </summary>
        /// <param name="hinge"></param>
        public static void JoinAnchors(this HingeJoint2D hinge)
        {
            hinge.connectedAnchor = hinge.connectedBody != null ? hinge.connectedBody.transform.InverseTransformPoint(hinge.transform.TransformPoint(hinge.anchor))
                : hinge.transform.TransformPoint(hinge.anchor);
        }
        public static void MoveAnchor(this HingeJoint2D hinge, Vector2 pos)
        {
            hinge.anchor = hinge.transform.InverseTransformPoint(pos);
        }
        public static void MoveConnectedAnchor(this HingeJoint2D hinge, Vector3 pos)
        {
            hinge.connectedAnchor = hinge.connectedBody != null ? hinge.connectedBody.transform.InverseTransformPoint(pos) : pos;
        }
        public static void ResetConnectedAnchor(this HingeJoint2D hinge)
        {
            if (hinge == null) return;
            hinge.connectedAnchor = hinge.connectedBody != null ? hinge.connectedBody.transform.InverseTransformPoint(hinge.transform.TransformPoint(hinge.anchor)) : hinge.transform.TransformPoint(hinge.anchor);
        }
        public static void SetLimits(this HingeJoint2D hinge, float min, float max, bool useLimits = true)
        {
            var limit = hinge.limits;
            limit.min = min;
            limit.max = max;
            hinge.limits = limit;
            hinge.useLimits = useLimits;
        }
        public static void SetMotor(this HingeJoint2D hinge, float torque, float speed, bool useMotor = true)
        {
            var motor = hinge.motor;
            motor.maxMotorTorque = torque;
            motor.motorSpeed = speed;
            hinge.motor = motor;
            hinge.useMotor = useMotor;
        }
        public static Vector3 GetWorldAnchor(this HingeJoint2D hinge)
        {
            var p = hinge.transform.TransformPoint(hinge.anchor);
            p.z = hinge.transform.position.z;
            return p;
        }
        /// <summary>
        /// Returns new HingeJoint2D with the same parameters. Used to interpolate limits correctly. Retruns same joint if limits are disabled.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static HingeJoint2D Reset(this HingeJoint2D h)
        {
            if (h.useLimits == false) return h;
            var limits = h.limits;
            var z = h.transform.localEulerAngles.z.To180();
            var euler = Vector3.zero;
            h.transform.localEulerAngles = euler;
            var motor = h.motor;
            bool useMotor = h.useMotor;
            var a = h.anchor;
            var ca = h.connectedAnchor;
            var cb = h.connectedBody;

            var g = h.gameObject;
            Object.Destroy(h);
            h = g.AddComponent<HingeJoint2D>();
            h.motor = motor;
            h.SetLimits(limits.min - z, limits.max - z);
            h.connectedBody = cb;
            h.connectedAnchor = ca;
            h.anchor = a;
            h.useMotor = useMotor;
            h.useLimits = true;
            euler.z = z;
            h.transform.localEulerAngles = euler;
            return h;
        }

        #endregion

        #region GameObject

        public static string GetInfo(this Rect rect)
        {
            return string.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", rect.x, rect.y, rect.width, rect.height);
        }

        public static bool ContainsComponent<T>(this GameObject g) where T : Component
        {
            return g.GetComponent<T>() != null || g.GetComponentInParent<T>() != null || g.GetComponentInChildren<T>() != null;
        }

        public static bool ContainsComponent<T1, T2>(this GameObject g)
        {
            return g.GetComponent<T1>() != null
                || g.GetComponentInParent<T1>() != null
                || g.GetComponentInChildren<T1>() != null
                || g.GetComponentInChildren<T2>() != null
                || g.GetComponentInParent<T2>() != null
                || g.GetComponent<T2>() != null;
        }

        public static bool ContainsComponent<T1, T2, T3>(this GameObject g)
        {
            return g.GetComponent<T1>() != null
                || g.GetComponentInParent<T1>() != null
                || g.GetComponentInChildren<T1>() != null
                || g.GetComponentInChildren<T2>() != null
                || g.GetComponentInParent<T2>() != null
                || g.GetComponent<T2>() != null
                || g.GetComponentInChildren<T3>() != null
                || g.GetComponentInParent<T3>() != null
                || g.GetComponent<T3>() != null;
        }
        public static T AddComponentIfNull<T>(this GameObject target) where T : Component
        {
            var t = target.GetComponent<T>();
            if (t == null)
            {
                return target.AddComponent<T>();
            }
            else return t;
        }
        public static HingeJoint2D AddHingeJoint(this GameObject body, Vector2 point, Rigidbody2D target, bool enableLimits = true)
        {
            var h = body.AddComponent<HingeJoint2D>();
            h.anchor = body.transform.InverseTransformPoint(point);
            h.connectedBody = target;
            h.JoinAnchors();
            h.useLimits = enableLimits;
            h.SetLimits(0, 0);
            return h;
        }
        public static HingeJoint2D AddHingeJoint(this GameObject body, Vector2 point, bool enableLimits = true)
        {
            var h = body.AddComponent<HingeJoint2D>();
            h.anchor = body.transform.InverseTransformPoint(point);
            h.JoinAnchors();
            h.useLimits = enableLimits;
            h.SetLimits(0, 0);
            return h;
        }

        #endregion

        #region Mesh

        //public static Vector2 GetPointFromUV(this SkinnedMeshRenderer mesh, Vector2 uvPoint, Vector2 size, Vector2 zero, Space space = Space.World)
        //{
        //    var res = new Vector2((uvPoint.x / size.x) + zero.x, (uvPoint.y / size.y) + zero.y);
        //    return space == Space.World ? (Vector2)mesh.transform.TransformPoint(res) : res;
        //}
        //public static Vector2 GetPointFromUV(this SkinnedMeshRenderer mesh, Vector2 uvPoint, Vector2 zero, int pixelsPerUnit, Space space = Space.World)
        //{
        //    var size = mesh.GetUVSize(pixelsPerUnit);
        //    return mesh.GetPointFromUV(uvPoint, size, zero, space);
        //}
        //public static Vector2 GetPointFromUV(this SkinnedMeshRenderer mesh, Vector2 uvPoint, int pixelsPerUnit, Space space = Space.World)
        //{
        //    var size = mesh.GetUVSize(pixelsPerUnit);
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetPointFromUV(uvPoint, size, zero, space);
        //}
        //public static Vector2 GetUVFromPoint(this SkinnedMeshRenderer mesh, Vector2 point, Vector2 size, Vector2 zero)
        //{
        //    var local = mesh.transform.InverseTransformPoint(point);
        //    return new Vector3((local.x - zero.x) * size.x, (local.y - zero.y) * size.y);
        //}
        //public static Vector2 GetUVFromLocalPoint(this SkinnedMeshRenderer mesh, Vector2 point, Vector2 size, Vector2 zero)
        //{
        //    return new Vector3((point.x - zero.x) * size.x, (point.y - zero.y) * size.y);
        //}
        //public static Vector2 GetUVFromPoint(this SkinnedMeshRenderer mesh, Vector2 point, Vector2 size)
        //{
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromPoint(point, size, zero);
        //}
        //public static Vector2 GetUVFromPoint(this SkinnedMeshRenderer mesh, Vector2 point, int pixelsPerUnit)
        //{
        //    var size = mesh.GetUVSize(pixelsPerUnit);
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromPoint(point, size, zero);
        //}
        //public static Vector2 GetUVFromLocalPoint(this SkinnedMeshRenderer mesh, Vector2 point, int pixelsPerUnit)
        //{
        //    var size = mesh.GetUVSize(pixelsPerUnit);
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromLocalPoint(point, size, zero);
        //}
        //public static Vector2 GetZeroUVPoint(this SkinnedMeshRenderer mf, Vector2 size, Space space = Space.World)
        //{
        //    var mesh = mf.GetMesh();
        //    var botUV = mesh.uv[0];
        //    var botVert = mesh.vertices[0];
        //    var zero = new Vector2(botVert.x - (botUV.x / size.x), botVert.y - (botUV.y / size.y));
        //    return space == Space.World ? (Vector2)mf.transform.TransformPoint(zero) : zero;
        //}
        //public static void SetUVs(this SkinnedMeshRenderer mf, Vector2 size, Vector2 zero)
        //{
        //    var mesh = mf.GetMesh();
        //    var vert = mesh.vertices;
        //    var uv = new Vector2[vert.Length];
        //    for (int i = 0; i < vert.Length; i++)
        //    {
        //        uv[i] = mf.GetUVFromLocalPoint(vert[i], size, zero);
        //    }
        //    mesh.uv = uv;
        //    mf.SetMesh(mesh);
        //}

        public static void AddBone(this SkinnedMeshRenderer skin, Vector3 point)
        {
            var bones = new List<Transform>(skin.bones);
            var bindPoses = new List<Matrix4x4>(skin.sharedMesh.bindposes);
            var bone = new GameObject("bone_" + bones.Count.ToString()).transform;
            bone.transform.SetParent(skin.transform);
            bone.transform.localPosition = point;
            bones.Add(bone);
            bindPoses.Add(bone.worldToLocalMatrix * skin.transform.localToWorldMatrix);
            skin.bones = bones.ToArray();
            skin.sharedMesh.bindposes = bindPoses.ToArray();

        }
        public static void AddBone(this SkinnedMeshRenderer skin, Vector3 point, Transform bone)
        {
            var bones = new List<Transform>(skin.bones);
            var bindPoses = new List<Matrix4x4>(skin.sharedMesh.bindposes);
            bone.transform.SetParent(skin.transform);
            bone.transform.localPosition = point;
            bones.Add(bone);
            bindPoses.Add(bone.worldToLocalMatrix * skin.transform.localToWorldMatrix);
            skin.bones = bones.ToArray();
            skin.sharedMesh.bindposes = bindPoses.ToArray();
        }
        public static void RemoveBone(this SkinnedMeshRenderer skin, Transform bone)
        {
            var bones = new List<Transform>(skin.bones);
            var bindPoses = new List<Matrix4x4>(skin.sharedMesh.bindposes);
            bones.Remove(bone);
            bindPoses.RemoveAt(bones.FindIndex((b) => { return b == bone; }));
            skin.sharedMesh.bindposes = bindPoses.ToArray();
            skin.bones = bones.ToArray();
        }
        public static Vector2 TextureSize(this MeshFilter mf)
        {
            var mesh = mf.GetMesh();
            var uvList = new List<Vector2>(mesh.uv);
            var result = new Vector2();
            uvList.Sort((o1, o2) => { return o1.x.CompareTo(o2.x); });
            result.x = uvList[uvList.Count - 1].x - uvList[0].x;
            uvList.Sort((o1, o2) => { return o1.y.CompareTo(o2.y); });
            result.y = uvList[uvList.Count - 1].y - uvList[0].y;
            return result;
        }
        public static Vector2 TextureSizeinPixels(this MeshFilter mf, Texture2D map)
        {
            var mesh = mf.GetMesh();
            var uvList = new List<Vector2>(mesh.uv);
            var result = new Vector2();
            uvList.Sort((o1, o2) => { return o1.x.CompareTo(o2.x); });
            result.x = (uvList[uvList.Count - 1].x - uvList[0].x) * map.width;
            uvList.Sort((o1, o2) => { return o1.y.CompareTo(o2.y); });
            result.y = (uvList[uvList.Count - 1].y - uvList[0].y) * map.height;
            return result;
        }
        public static void ResetUV(this MeshFilter mf)
        {
            var mesh = mf.GetMesh();
            var uv = mesh.uv;
            uv[0] = new Vector2(1, 0);
            uv[1] = new Vector2(0, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;
            mf.SetMesh(mesh);
        }
        public static Mesh GetMesh(this MeshFilter mf)
        {
            return Application.isPlaying ? mf.mesh : mf.sharedMesh;
        }
        public static Mesh GetMesh(this SkinnedMeshRenderer mf)
        {
            return mf.sharedMesh;
        }
        public static void SetMesh(this MeshFilter mf, Mesh mesh)
        {
            if (Application.isPlaying) mf.mesh = mesh;
            else mf.sharedMesh = mesh;
        }
        public static void SetMesh(this SkinnedMeshRenderer mf, Mesh mesh)
        {
            mf.sharedMesh = mesh;
        }
        public static void SetPivot(this MeshFilter mf, Vector3 pivot)
        {
            var mesh = mf.GetMesh();
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= pivot;
            }
            foreach (Transform child in mf.transform)
            {
                child.localPosition -= pivot;
            }
            mf.SendMessage("MovePivot", pivot, SendMessageOptions.DontRequireReceiver);
            mf.SetVertices(vertices);
            mf.transform.position = mf.transform.TransformPoint(pivot);
        }
        public static Vector2 GetUVSize(this MeshRenderer mf, int pixelsPerUnit)
        {
            var rect = new Rect(0, 0, mf.material.mainTexture.width, mf.material.mainTexture.height);
            float ppu = pixelsPerUnit;
            return new Vector2(ppu / rect.width, ppu / rect.height);
        }
        public static Vector2 GetUVSize(this SkinnedMeshRenderer mf, int pixelsPerUnit)
        {
            var rect = new Rect(0, 0, mf.material.mainTexture.width, mf.material.mainTexture.height);
            float ppu = pixelsPerUnit;
            return new Vector2(ppu / rect.width, ppu / rect.height);
        }
        //public static Vector2 GetPointFromUV(this MeshFilter mesh, Vector2 uvPoint, Vector2 size, Vector2 zero, Space space = Space.World)
        //{
        //    var res = new Vector2((uvPoint.x / size.x) + zero.x, (uvPoint.y / size.y) + zero.y);
        //    return space == Space.World ? (Vector2)mesh.transform.TransformPoint(res) : res;
        //}

        //public static Vector2 GetPointFromUV(this MeshFilter mesh, Vector2 uvPoint, int pixelsPerUnit, Space space = Space.World)
        //{
        //    var size = mesh.GetUVSize(pixelsPerUnit);
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetPointFromUV(uvPoint, size, zero, space);
        //}
        //public static Vector2 GetUVFromPoint(this MeshFilter mesh, Vector2 point, int pixelsPerUnit, Vector2 zero)
        //{
        //    var local = mesh.transform.InverseTransformPoint(point);
        //    return new Vector3((local.x - zero.x) * size.x, (local.y - zero.y) * size.y);
        //}
        //public static Vector2 GetUVFromLocalPoint(this MeshFilter mesh, Vector2 point, Vector2 size, Vector2 zero)
        //{
        //    return new Vector2((point.x - zero.x) * size.x, (point.y - zero.y) * size.y);
        //}
        //public static Vector2 GetUVFromPoint(this MeshFilter mesh, Vector2 point, Vector2 size)
        //{
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromPoint(point, size, zero);
        //}
        //public static Vector2 GetUVFromPoint(this MeshFilter mesh, Vector2 point)
        //{
        //    var size = mesh.GetUVSize();
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromPoint(point, size, zero);
        //}
        //public static Vector2 GetUVFromLocalPoint(this MeshFilter mesh, Vector2 point)
        //{
        //    var size = mesh.GetUVSize();
        //    var zero = mesh.GetZeroUVPoint(size, Space.Self);
        //    return mesh.GetUVFromLocalPoint(point, size, zero);
        //}
        //public static Vector2 GetZeroUVPoint(this MeshFilter mf, Vector2 size, Space space = Space.World)
        //{
        //    var mesh = mf.GetMesh();
        //    var botUV = mesh.uv[0];
        //    var botVert = mesh.vertices[0];
        //    var zero = new Vector2(botVert.x - (botUV.x / size.x), botVert.y - (botUV.y / size.y));
        //    return space == Space.World ? (Vector2)mf.transform.TransformPoint(zero) : zero;
        //}
        //public static void SetUVs(this MeshFilter mf, Vector2 size, Vector2 zero)
        //{
        //    var mesh = mf.GetMesh();
        //    var vert = mesh.vertices;
        //    var uv = new Vector2[vert.Length];
        //    for (int i = 0; i < vert.Length; i++)
        //    {
        //        uv[i] = mf.GetUVFromLocalPoint(vert[i], size, zero);
        //    }
        //    mesh.uv = uv;
        //    mf.SetMesh(mesh);
        //}
        public static void FlipVertices(this MeshFilter mf, Vector2 direction)
        {
            var m = mf.GetMesh();
            var vertices = m.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector2.Reflect(vertices[i], direction);
            }
            m.vertices = vertices;
        }
        public static void Flip(this PolygonCollider2D polygon, Vector2 direction)
        {
            var points = polygon.points;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Vector2.Reflect(points[i], direction);
            }
            polygon.SetPath(0, points);
        }
        public static void Flip(this HingeJoint2D hinge, Vector2 direction)
        {
            hinge.connectedAnchor = Vector2.Reflect(hinge.connectedAnchor, direction);
            hinge.anchor = Vector2.Reflect(hinge.anchor, direction);
            if (hinge.useLimits)
            {
                hinge.SetLimits(hinge.limits.min * -1, hinge.limits.max * -1);
            }
        }
        public static void FlipVertices(this MeshFilter mf)
        {
            var m = mf.GetMesh();
            var vertices = m.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector2.Reflect(vertices[i], Vector2.left);
            }
            m.vertices = vertices;
        }
        public static void Flip(this PolygonCollider2D polygon)
        {
            var points = polygon.points;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Vector2.Reflect(points[i], Vector2.left);
            }
            polygon.SetPath(0, points);
        }
        public static void Flip(this HingeJoint2D hinge, float offset = 0)
        {
            hinge.connectedAnchor = Vector2.Reflect(hinge.connectedAnchor, Vector2.left);
            hinge.anchor = Vector2.Reflect(hinge.anchor, Vector2.left);
            if (hinge.useMotor)
                hinge.SetMotor(hinge.motor.maxMotorTorque, hinge.motor.motorSpeed * -1);
            if (hinge.useLimits)
            {
                hinge.SetLimits(hinge.limits.min * -1, hinge.limits.max * -1);
            }
            if (hinge.connectedBody == null) hinge.JoinAnchors();
        }
        public static void FlipNormal(this MeshRenderer mr)
        {
            mr.material.SetFloat("_BumpFlip", mr.transform.localEulerAngles.y > 0 ? -1 : 1);
        }
        public static void SetVertices(this MeshFilter mf, Vector3[] vertices)
        {
            Mesh mesh = mf.GetMesh();

            //  Triangulator tr = new Triangulator(vertices);
            //  int[] triangles = tr.Triangulate();
            if (mesh.vertices.Length < vertices.Length)
            {
                mesh.vertices = vertices;
                // mesh.triangles = triangles;
            }
            else
            {
                // mesh.triangles = triangles;
                mesh.vertices = vertices;
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mf.SetMesh(mesh);
        }
        public static Mesh Copy(this Mesh mesh)
        {
            return Object.Instantiate(mesh);
        }
        public static void SetVertices(this SkinnedMeshRenderer mf, Vector3[] vertices)
        {
            Mesh mesh = mf.GetMesh();

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mf.SetMesh(mesh);
        }
        public static BoneWeight NormalizedWeight(this BoneWeight original, BoneWeight edited)
        {
            float[] editedWeights = new float[] { edited.weight0, edited.weight1 };
            float[] originalWeights = new float[] { original.weight0, original.weight1 };
            float[] result = Helper.KeepSum(1, editedWeights, originalWeights);
            original.weight0 = Mathf.Clamp(result[0], 0, 1);
            original.weight1 = Mathf.Clamp(result[1], 0, 1);
            original.weight2 = 0;
            original.weight3 = 0;
            return original;
        }
        #endregion

        #region GameObject


        #endregion

        #region Sprite

        public static float GetAspect(this Sprite sprite)
        {
            return sprite.rect.width / sprite.rect.height;
        }

        #endregion Sprite

        #region SkinnedMesh

        public static void AttachBones(this SkinnedMeshRenderer skin, Transform[] bones)
        {
            var bp = new Matrix4x4[bones.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                bp[i] = bones[i].transform.worldToLocalMatrix * skin.transform.localToWorldMatrix;
            }
            skin.bones = bones;
            skin.FixBoneWeights();
            skin.sharedMesh.bindposes = bp;
        }
        public static bool FixBoneWeights(this SkinnedMeshRenderer skin)
        {
            var weights = skin.sharedMesh.boneWeights;
            var boneCount = skin.bones.Length;
            bool hasIssue = false;
            for (int i = 0; i < weights.Length; i++)
            {
                var bw = weights[i];
                if (bw.boneIndex0 >= boneCount)
                {
                    Debug.LogWarning("Bone index " + i.ToString() + " is out of bounds. Fixing...");
                    bw.boneIndex0 = 0;
                    bw.weight0 = 0;
                    hasIssue = true;
                }
                if (bw.boneIndex1 >= boneCount)
                {
                    Debug.LogWarning("Bone index " + i.ToString() + " is out of bounds. Fixing...");
                    bw.boneIndex1 = 0;
                    bw.weight1 = 0;
                    hasIssue = true;
                }
                if (bw.boneIndex2 >= boneCount)
                {
                    Debug.LogWarning("Bone index " + i.ToString() + " is out of bounds. Fixing...");
                    bw.boneIndex2 = 0;
                    bw.weight2 = 0;
                    hasIssue = true;
                }
                if (bw.boneIndex3 >= boneCount)
                {
                    Debug.LogWarning("Bone index " + i.ToString() + " is out of bounds. Fixing...");
                    bw.boneIndex3 = 0;
                    bw.weight3 = 0;
                    hasIssue = true;
                }
                weights[i] = bw;
            }
            skin.sharedMesh.boneWeights = weights;
            return hasIssue;
        }
        public static bool IsBoneWeightsBroken(this SkinnedMeshRenderer skin)
        {
            var weights = skin.sharedMesh.boneWeights;
            var boneCount = skin.bones.Length;
            for (int i = 0; i < weights.Length; i++)
            {
                var bw = weights[i];
                if (bw.boneIndex0 >= boneCount)
                {
                    return true;
                }
                if (bw.boneIndex1 >= boneCount)
                {
                    return true;
                }
                if (bw.boneIndex2 >= boneCount)
                {
                    return true;
                }
                if (bw.boneIndex3 >= boneCount)
                {
                    return true;
                }
                weights[i] = bw;
            }
            return false;
        }
        public static void DetachBones(this SkinnedMeshRenderer skin)
        {
            var bp = new Matrix4x4[0];
            var bones = new Transform[0];
            skin.bones = bones;
            skin.sharedMesh.bindposes = bp;
        }
        #endregion

        #region Rigidbody2D

        public static void MoveWithOffset(this Rigidbody2D target, Vector3 targetPosition, Vector3 offset)
        {
            targetPosition.x = targetPosition.x + (target.transform.position.x - offset.x);
            targetPosition.y = targetPosition.y + (target.transform.position.y - offset.y);
            target.MovePosition(targetPosition);
        }

        public static void AddTorqueTowardsLocal(this Rigidbody2D body, Transform t, float angle, float gainConstant)
        {
            var z = t.localEulerAngles.z;
            angle = Mathf.LerpAngle(z, angle, 360f);
            body.AddTorque((z - angle) * -gainConstant - body.angularVelocity);
        }
        public static void AddTorqueTowards(this Rigidbody2D body, Transform t, float angle, float gainConstant)
        {
            var z = t.eulerAngles.z;
            angle = Mathf.LerpAngle(z, angle, 360f);
            body.AddTorque((z - angle) * -gainConstant - body.angularVelocity);
        }
        public static void AddNormalizedTorque(this Rigidbody2D body, float angle, float gainConstant)
        {
            body.AddTorque((body.rotation - (Helper.To180Angle(angle)) * -gainConstant - body.angularVelocity));
        }
        /// <summary>
        /// Throws body to the target vector.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="target"> Target point in worlds space. </param>
        /// <param name="gainConstant"> Force applied to the body. </param>
        /// <param name="distanceMultipiller"> Whether or nor distance between body and target is giving additional force. </param>
        public static void AddForceTowards(this Rigidbody2D body, Vector2 target, float gainConstant, bool distanceMultipiller = true)
        {
            body.AddForce(((target - (Vector2) body.transform.TransformPoint(body.centerOfMass)).normalized * (gainConstant * body.mass * (distanceMultipiller ? Vector2.Distance(body.position, target) : 1))));
        }
        public static void AddForceTowards(this Rigidbody2D body, Vector2 target, float gainConstant, Vector2 dragPoint, bool distanceMultipiller = true)
        {
            body.AddForce(((target - (Vector2) body.transform.TransformPoint(dragPoint)).normalized * (gainConstant * body.mass * (distanceMultipiller ? Vector2.Distance(body.position, target) : 1))));
        }



        #endregion

        #region Arrays and Lists

        public static int PreviousIndex<T>(this List<T> list, int index, int offset = 0)
        {
            while (offset > 0)
            {
                index = list.PreviousIndex(index);
                offset--;
            }
            return index > 0 ? index - 1 : list.Count - 1;
        }

        public static List<T> ToList<T>(this T[] array)
        {
            var length = array.Length;
            var list = new List<T>(length);
            for (int i = 0; i < length; i++)
            {
                list.Add(array[i]);
            }
            return list;
        }

        public static int NextIndex<T>(this List<T> list, int index, int offset = 0)
        {
            while (offset > 0)
            {
                index = list.NextIndex(index);
                offset--;
            }
            return index < list.Count - 1 ? index + 1 : 0;
        }
        public static int PreviousIndex<T>(this T[] list, int index, int offset = 0)
        {
            while (offset > 0)
            {
                index = list.PreviousIndex(index);
                offset--;
            }
            return index > 0 ? index - 1 : list.Length - 1;
        }
        public static int NextIndex<T>(this T[] list, int index, int offset = 0)
        {
            while (offset > 0)
            {
                index = list.NextIndex(index);
                offset--;
            }
            return index < list.Length - 1 ? index + 1 : 0;
        }
        public static int BetweenCount<T>(this List<T> list, int a, int b, bool clockwise)
        {
            int result = 0;
          //  a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            while (a != b)
            {
                result++;
                a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            }
            return result;
        }
        public static int BetweenCount<T>(this T[] list, int a, int b, bool clockwise)
        {
            int result = 0;
          //  a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            while (a != b)
            {
                result++;
                a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            }
            return result;
        }
        public static List<T> BetweenItems<T>(this List<T> list, int a, int b, bool clockwise)
        {
            var result = new List<T>();
            a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            while (a != b)
            {
                result.Add(list[a]);
                a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            }
            return result;
        }
        public static List<T> BetweenItems<T>(this List<T> list, int a, int b, bool clockwise, ref List<T> otherHalf)
        {
            var result = new List<T>();
            a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            while (a != b)
            {
                result.Add(list[a]);
                a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            }
            return result;
        }
        public static List<T> BetweenItems<T>(this List<T> list, int a, int b)
        {
            var result = new List<T>();
            var clockwise = list.BetweenCount(a, b, true) < list.BetweenCount(a, b, false);
            a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            while (a != b)
            {
                result.Add(list[a]);
                a = clockwise ? list.NextIndex(a) : list.PreviousIndex(a);
            }
            return result;
        }
        public static T NextItem<T>(this List<T> list, int index)
        {
            if (list.Count == 0) return default(T);
            var i = index < list.Count - 1 ? index + 1 : 0;
            return list[i];
        }
        public static T PreviousItem<T>(this List<T> list, int index)
        {
            var i = index > 0 ? index - 1 : list.Count - 1;
            return list[i];
        }
        public static T NextItem<T>(this T[] list, int index)
        {
            if (list.Length == 0) return default(T);
            var i = index < list.Length - 1 ? index + 1 : 0;
            return list[i];
        }
        public static T PreviousItem<T>(this T[] list, int index)
        {
            if (list.Length == 0) return default(T);
            var i = index > 0 ? index - 1 : list.Length - 1;
            return list[i];
        }
        public static T LastItem<T>(this T[] list)
        {
            if (list.Length == 0) return default(T);
            return list[list.Length - 1];
        }
        public static T LastItem<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[list.Count - 1];
        }
        public static T Random<T>(this T[] list)
        {
            return list[UnityEngine.Random.Range(0, list.Length)];
        }
        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T[] Add<T>(this T[] array, T item)
        {
            System.Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
            return array;
        }
        public static void Shuffle<T>(this T[] array)
        {
            System.Array.Sort(array, (i1, i2) => UnityEngine.Random.Range(-1, 2));
        }
        public static void Shuffle<T>(this List<T> list)
        {
            list.Sort((i1, i2) => UnityEngine.Random.Range(-1, 2));
        }
        #endregion

        #region Vectors

        /// <summary>
        /// Converts each point to selected space based on parent.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="space"></param>
        /// Space relative to the parent.
        /// <param name="parent"></param>
        /// Parent transform used to convert from local to world and in reverse.
        /// <returns></returns>
        public static Vector2[] Convert(this Vector2[] points, Space space, Transform parent)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = space == Space.Self ? parent.InverseTransformPoint(points[i]) : parent.TransformPoint(points[i]);
            }
            return points;
        }
        /// <summary>
        /// Converts each point to selected space based on parent.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="space"></param>
        /// Space relative to the parent.
        /// <param name="parent"></param>
        /// Parent transform used to convert from local to world and in reverse.
        /// <returns></returns>
        public static Vector3[] Convert(this Vector3[] points, Space space, Transform parent)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = space == Space.Self ? parent.InverseTransformPoint(points[i]) : parent.TransformPoint(points[i]);
            }
            return points;
        }
        /// <summary>
        /// Converts each point to selected space based on parent.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="space"></param>
        /// Space relative to the parent.
        /// <param name="parent"></param>
        /// Parent transform used to convert from local to world and in reverse.
        /// <returns></returns>
        public static List<Vector2> Convert(this List<Vector2> points, Space space, Transform parent)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = space == Space.Self ? parent.InverseTransformPoint(points[i]) : parent.TransformPoint(points[i]);
            }
            return points;
        }
        /// <summary>
        /// Converts each point to selected space based on parent.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="space"></param>
        /// Space relative to the parent.
        /// <param name="parent"></param>
        /// Parent transform used to convert from local to world and in reverse.
        /// <returns></returns>
        public static List<Vector3> Convert(this List<Vector3> points, Space space, Transform parent)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = space == Space.Self ? parent.InverseTransformPoint(points[i]) : parent.TransformPoint(points[i]);
            }
            return points;
        }
        public static Vector2[] ToVector2Array(this Vector3[] array)
        {
            var res = new Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                res[i] = new Vector2(array[i].x, array[i].y);
            }
            return res;
        }
        public static Vector3[] ToVector3Array(this Vector2[] array)
        {
            var res = new Vector3[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                res[i] = new Vector3(array[i].x, array[i].y, 0);
            }
            return res;
        }
        public static List<Vector2> Flip(this List<Vector2> v)
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] = Vector2.Reflect(v[i], Vector2.left);
            }
            return v;
        }
        public static List<Vector2> Flip(this List<Vector2> v, Vector2 normal)
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] = Vector2.Reflect(v[i], normal);
            }
            return v;
        }
        public static List<Vector3> Flip(this List<Vector3> v)
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] = Vector3.Reflect(v[i], Vector2.left);
            }
            return v;
        }
        public static List<Vector3> Flip(this List<Vector3> v, Vector2 normal)
        {
            for (int i = 0; i < v.Count; i++)
            {
                v[i] = Vector3.Reflect(v[i], normal);
            }
            return v;
        }
        public static Vector2 Flip(this Vector2 v, Vector2 normal)
        {
            return Vector2.Reflect(v, normal);
        }
        public static Vector3 Flip(this Vector3 v)
        {
            return Vector3.Reflect(v, Vector2.left);
        }
        public static Vector3 Flip(this Vector3 v, Vector2 normal)
        {
            return Vector3.Reflect(v, normal);
        }
        public static Vector2[] Flip(this Vector2[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = Vector2.Reflect(v[i], Vector2.left);
            }
            return v;
        }
        public static Vector2[] Flip(this Vector2[] v, Vector2 normal)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = Vector2.Reflect(v[i], normal);
            }
            return v;
        }
        public static Vector3[] Flip(this Vector3[] v, Vector2 normal)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = Vector3.Reflect(v[i], normal);
            }
            return v;
        }
        public static Vector3[] Flip(this Vector3[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = Vector3.Reflect(v[i], Vector2.left);
            }
            return v;
        }
        public static Vector3[] Shift(this Vector3[] v, Vector3 offset)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] -= offset;
            }
            return v;
        }
        public static Vector2[] Shift(this Vector2[] v, Vector2 offset)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] -= offset;
            }
            return v;
        }
        public static Vector2[] Scale(this Vector2[] v, float scale)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= scale;
            }
            return v;
        }
        public static Vector3[] Scale(this Vector3[] v, float scale)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= scale;
            }
            return v;
        }
        public static Vector2 Flip(this Vector2 v)
        {
            return Vector2.Reflect(v, Vector2.left);
        }
        public static string Print(this Vector2 v)
        {
            return string.Format("x: {0}, y: {1}", v.x.ToString("0.00"), v.y.ToString("0.00"));
        }
        public static string PrintAll(this Vector2 v)
        {
            return string.Format("x: {0}, y: {1}", v.x, v.y);
        }
        public static string PrintAll(this Vector3 v)
        {
            return string.Format("x: {0}, y: {1}, z: {2}", v.x, v.y, v.z);
        }
        public static string Print(this Vector3 v)
        {
            return string.Format("x: {0}, y: {1}, z: {2}", v.x.ToString("0.00"), v.y.ToString("0.00"), v.z.ToString("0.00"));
        }
        public static Vector2 SetX(this Vector2 v, float x)
        {
            v.x = x;
            return v;
        }
        public static Vector3 SetZ(this Vector2 v, float z)
        {
            var v3 = (Vector3) v;
            v3.z = z;
            return v3;
        }
        public static Vector2 SetY(this Vector2 v, float y)
        {
            v.y = y;
            return v;
        }
        public static Vector3 SetX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }
        public static Vector3 SetY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }
        public static Vector3 SetZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }
        #endregion

        #region Structs

        public static Color SetAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }
        public static Color Randomize(this Color c)
        {
            c.r = UnityEngine.Random.Range(0f, 1f);
            c.g = UnityEngine.Random.Range(0f, 1f);
            c.b = UnityEngine.Random.Range(0f, 1f);
            return c;
        }
        /// <summary>
        /// Interpolates float wrapping it around 180 angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float To180(this float angle)
        {
            while (angle < -180.0f) angle += 360.0f;
            while (angle >= 180.0f) angle -= 360.0f;
            return angle;
        }
        /// <summary>
        /// Interpolates float wrapping it around #60 angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float To360(this float angle)
        {
            while (angle < -0) angle += 360.0f;
            while (angle >= 360.0f) angle -= 360.0f;
            return angle;
        }

        #endregion

        #region Box Collider2D

        public static bool Slice(this BoxCollider2D box, Vector3 point, ref Vector2 sliceSize, ref Vector2 sliceOffset, bool sliceX, float minSize = 0.2f)
        {
            var closest = box.transform.InverseTransformPoint(box.bounds.ClosestPoint(point));
            if (sliceX)
            {
                var mainSize = (box.size.x / 2) + (closest.x - box.offset.x);
                if (mainSize < minSize) return false;
                sliceSize.x = box.size.x - mainSize;
                if (sliceSize.x < minSize) return false;
                sliceOffset.x = box.offset.x + (mainSize / 2);
                box.size = new Vector2(mainSize, box.size.y);
                box.offset = new Vector2(box.offset.x - (sliceSize.x / 2), box.offset.y);
            }
            else
            {
                var mainSize = (box.size.y / 2) + (closest.y - box.offset.y);
                if (mainSize < minSize) return false;
                sliceSize.y = box.size.y - mainSize;
                if (sliceSize.y < minSize) return false;
                sliceOffset.y = box.offset.y + (mainSize / 2);
                box.size = new Vector2(box.size.x, mainSize);
                box.offset = new Vector2(box.offset.x, box.offset.y - (sliceSize.y / 2));
            }
            return true;

        }
        public static bool Slice(this BoxCollider2D box, Vector3 point, ref Vector2 sliceSize, ref Vector2 sliceOffset)
        {
            var sliceX = box.size.x > box.size.y;
            return box.Slice(point, ref sliceSize, ref sliceOffset, sliceX);
        }
        public static Vector3 GetMouse(this GameObject m)
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        #endregion

        #region MonoBehaviour

        public static string GetScriptPath(this MonoBehaviour b)
        {
#if UNITY_EDITOR
            var ms = UnityEditor.MonoScript.FromMonoBehaviour(b);
            var scriptFilePath = UnityEditor.AssetDatabase.GetAssetPath(ms);
            return scriptFilePath;
#else
             return "";
#endif
        }

        public static T FindComponentInChild<T>(this MonoBehaviour t, string name)
        {
            var child = t.transform.Find(name);
            if (child)
            {
                return child.GetComponent<T>();
            }
            else return default(T);
        }

        #endregion MonoBehaviour

        #region Other

        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) != 0;
        }

        public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        #endregion Other

        #region Camera

        public static Vector2 GetSize(this Camera cam)
        {
            var s = 2f * cam.orthographicSize;
            return new Vector2(cam.aspect * s, s);
        }

        public static Bounds GetBounds(this Camera cam)
        {
            return new Bounds(cam.transform.position, cam.GetSize());
        }

        #endregion Camera

        #region RectTransform

        public static void SetPivotFromSprite(this RectTransform rect, Sprite sprite)
        {
            var pivot = sprite.pivot;
            pivot.x /= sprite.rect.width;
            pivot.y /= sprite.rect.height;
            rect.pivot = pivot;
        }

        public static Vector2 GetLocalPivotPoint(this RectTransform rect, float scale = 1f)
        {
            return Vector3.Scale(rect.sizeDelta, rect.pivot);
        }

        public static Vector2 GetPivotPoint(this RectTransform rect, float scale = 1f)
        {
            return rect.position + Vector3.Scale(rect.sizeDelta * scale, rect.pivot - rect.anchorMax);
        }

        public static Vector2 GetAnchoredPivotPoint(this RectTransform rect, float scale = 1f)
        {
            return rect.anchoredPosition + Vector2.Scale(rect.sizeDelta * scale, rect.pivot);
        }

        public static void SetAnchorFromSprite(this RectTransform rect, Sprite sprite)
        {
            var pivot = sprite.pivot;
            pivot.x /= sprite.rect.width;
            pivot.y /= sprite.rect.height;
            rect.anchorMax = pivot;
            rect.anchorMin = pivot;
        }

        public static Vector3 GetWorldPosition(this RectTransform rect, Camera cam)
        {
            return cam.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(null, rect.position));
        }

        public static Vector3 GetWorldPosition(this RectTransform rect)
        {
            return Camera.main.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(null, rect.position));
        }

        #endregion RectTransform

        #region Rigidbody

        public static void AddTorqueTowardsLocal(this Rigidbody body, Transform t, Quaternion angle, float gainConstant)
        {
            var z = t.localRotation;
            angle = Quaternion.Lerp(z, angle, 360f);
            body.AddTorque((z.eulerAngles - angle.eulerAngles) * -gainConstant - body.angularVelocity);
        }
        public static void AddTorqueTowards(this Rigidbody body, Transform t, Vector3 angle, float gainConstant)
        {
            var z = t.localEulerAngles;
            
            angle.x = Mathf.LerpAngle(z.x, angle.x, 360);
            angle.y = Mathf.LerpAngle(z.y, angle.y, 360);
            angle.z = Mathf.LerpAngle(z.z, angle.z, 360);

            body.AddTorque((z - angle) * -gainConstant - body.angularVelocity);
        }

        #endregion Rigidbody

        #region Conf Joint

        /// <summary>
        /// Sets a joint's targetRotation to match a given local rotation.
        /// The joint transform's local rotation must be cached on Start and passed into this method.
        /// </summary>
        public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
        {
            if (joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
            }
            SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
        }

        /// <summary>
        /// Sets a joint's targetRotation to match a given world rotation.
        /// The joint transform's world rotation must be cached on Start and passed into this method.
        /// </summary>
        public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
        {
            if (!joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
            }
            SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
        }

        static void SetTargetRotationInternal(this ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
        {
            // Calculate the rotation expressed by the joint's axis and secondary axis
            var right = joint.axis;
            var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            var up = Vector3.Cross(forward, right).normalized;
            Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

            // Transform into world space
            Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

            // Counter-rotate and apply the new local rotation.
            // Joint space is the inverse of world space, so we need to invert our value
            if (space == Space.World)
            {
                resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
            }
            else
            {
                resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
            }

            // Transform back into joint space
            resultRotation *= worldToJointSpace;

            // Set target rotation to our newly calculated rotation
            joint.targetRotation = resultRotation;
        }

        #endregion Conf Joint

        #region Texture 2D

        public static void Fill(this Texture2D texture, int x, int y, Color32 replacementColor)
        {
            var target = texture.GetPixel(x, y);
            Flood(texture, target, replacementColor, x, y, texture.width, texture.height);
            texture.Apply();
        }

        static void Flood(this Texture2D texture, Color target, Color replacement, int x, int y, int w, int h)
        {
            if (x < 0 || x >= w || y < 0 || y >= h) return;
            if (target == replacement) return;
            var color = texture.GetPixel(x, y);
            if (color != target) return;
            texture.SetPixel(x, y, replacement);

            Flood(texture, target, replacement, x, y - 1, w, h);
            Flood(texture, target, replacement, x - 1, y, w, h);
            Flood(texture, target, replacement, x + 1, y, w, h);
            Flood(texture, target, replacement, x, y + 1, w, h);
        }


        #endregion Texture 2D

        #region Configurable Joint

        public static void LockAll(this ConfigurableJoint j)
        {
            j.xMotion = ConfigurableJointMotion.Locked;
            j.yMotion = ConfigurableJointMotion.Locked;
            j.zMotion = ConfigurableJointMotion.Locked;
            j.angularXMotion = ConfigurableJointMotion.Locked;
            j.angularYMotion = ConfigurableJointMotion.Locked;
            j.angularZMotion = ConfigurableJointMotion.Locked;
        }

        public static void UnlockAll(this ConfigurableJoint j)
        {
            j.xMotion = ConfigurableJointMotion.Free;
            j.yMotion = ConfigurableJointMotion.Free;
            j.zMotion = ConfigurableJointMotion.Free;
            j.angularXMotion = ConfigurableJointMotion.Free;
            j.angularYMotion = ConfigurableJointMotion.Free;
            j.angularZMotion = ConfigurableJointMotion.Free;
        }

        public static ConfigurableJoint ResetLocked(this ConfigurableJoint j)
        {
            var connected = j.connectedBody;
            var a = j.axis;
            var ca = j.connectedAnchor;
            var e = j.enableCollision;
            var ep = j.enablePreprocessing;

            var g = j.gameObject;

            Object.Destroy(j);

            j = g.AddComponent<ConfigurableJoint>();
            j.connectedBody = connected;
            j.axis = a;
            j.connectedAnchor = ca;
            j.enableCollision = e;
            j.enablePreprocessing = ep;
            j.LockAll();
          
            return j;
        }

        public static bool IsLocked(this ConfigurableJoint j)
        {
            return j.xMotion == ConfigurableJointMotion.Locked
                && j.yMotion == ConfigurableJointMotion.Locked
                && j.zMotion == ConfigurableJointMotion.Locked
                && j.angularXMotion == ConfigurableJointMotion.Locked
                && j.angularYMotion == ConfigurableJointMotion.Locked
                && j.angularZMotion == ConfigurableJointMotion.Locked;
        }

        public static ConfigurableJoint Reset(this ConfigurableJoint j)
        {
            var connectedBody = j.connectedBody;
            var angularXDrive = j.angularXDrive;
            var angularXLimitSpring = j.angularXLimitSpring;
            var angularXMotion = j.angularXMotion;
            var angularYLimit = j.angularYLimit;
            var angularYMotion = j.angularYMotion;
            var angularYZDrive = j.angularYZDrive;
            var angularYZLimitSpring = j.angularYZLimitSpring;
            var angularZLimit = j.angularZLimit;
            var angularZMotion = j.angularZMotion;
            var axis = j.axis;
            var anchor = j.anchor;
            var connectedAnchor = j.connectedAnchor;
            var breakForce = j.breakForce;
            var breakTorque = j.breakTorque;
            var configuredInWorldSpace = j.configuredInWorldSpace;
            var autoConfigureConnectedAnchor = j.autoConfigureConnectedAnchor;
            var connectedMassScale = j.connectedMassScale;
            var enableCollision = j.enableCollision;
            var enablePreprocessing = j.enablePreprocessing;
            var highAngularXLimit = j.highAngularXLimit;
            var linearLimit = j.linearLimit;
            var linearLimitSpring = j.linearLimitSpring;
            var lowAngularXLimit = j.lowAngularXLimit;
            var massScale = j.massScale;
            var projectionAngle = j.projectionAngle;
            var projectionDistance = j.projectionDistance;
            var projectionMode = j.projectionMode;
            var rotationDriveMode = j.rotationDriveMode;
            var secondaryAxis = j.secondaryAxis;
            var slerpDrive = j.slerpDrive;
            var swapBodies = j.swapBodies;
            var targetAngularVelocity = j.targetAngularVelocity;
            var targetPosition = j.targetPosition;
            var targetRotation = j.targetRotation;
            var targetVelocity = j.targetVelocity;
            var xDrive = j.xDrive;
            var xMotion = j.xMotion;
            var yDrive = j.yDrive;
            var yMotion = j.yMotion;
            var zDrive = j.zDrive;
            var zMotion = j.zMotion;

            var g = j.gameObject;

            Object.Destroy(j);
          
            j = g.AddComponent<ConfigurableJoint>();
            j.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
            j.anchor = anchor;
            j.connectedBody = connectedBody;
            j.angularXDrive = angularXDrive;
            j.angularXLimitSpring = angularXLimitSpring;
            j.angularXMotion = angularXMotion;
            j.angularYLimit = angularYLimit;
            j.angularYMotion = angularYMotion;
            j.angularYZDrive = angularYZDrive;
            j.angularYZLimitSpring = angularYZLimitSpring;
            j.angularZLimit = angularZLimit;
            j.angularZMotion = angularZMotion;
            j.axis = axis;
            j.breakForce = breakForce;
            j.breakTorque = breakTorque;
            j.configuredInWorldSpace = configuredInWorldSpace;
            j.connectedAnchor = connectedAnchor;
            j.connectedMassScale = connectedMassScale;
            j.enableCollision = enableCollision;
            j.enablePreprocessing = enablePreprocessing;
            j.highAngularXLimit = highAngularXLimit;
            j.linearLimit = linearLimit;
            j.linearLimitSpring = linearLimitSpring;
            j.lowAngularXLimit = lowAngularXLimit;
            j.massScale = massScale;
            j.projectionAngle = projectionAngle;
            j.projectionDistance = projectionDistance;
            j.projectionMode = projectionMode;
            j.rotationDriveMode = rotationDriveMode;
            j.secondaryAxis = secondaryAxis;
            j.slerpDrive = slerpDrive;
            j.swapBodies = swapBodies;
            j.targetAngularVelocity = targetAngularVelocity;
            j.targetPosition = targetPosition;
            j.targetRotation = targetRotation;
            j.targetVelocity = targetVelocity;
            j.xDrive = xDrive;
            j.xMotion = xMotion;
            j.yDrive = yDrive;
            j.yMotion = yMotion;
            j.zDrive = zDrive;
            j.zMotion = zMotion;
            return j;
        }

        #endregion Configurable Joint

        #region Animation Curves

        public static string ToCodeString(this Keyframe k)
        {
            return string.Format("new Keyframe() {{ time = {4}f, value = {0}f, inTangent = {1}f,  outTangent = {2}f, tangentMode = {3} }}", k.value, k.inTangent, k.outTangent, k.tangentMode, k.time);
        }

        public static string ToCodeString(this Keyframe[] keys, string indent = "")
        {
            var s = "";
            foreach (var k in keys)
            {
                s += indent + k.ToCodeString() + ",";
                s += '\n';
            }
            s = s.Remove(s.Length - 1, 1);
            return s;
        }

        public static string ToCodeString(this AnimationCurve curve)
        {
            return @"public AnimationCurve curve = new AnimationCurve()
{
    keys = new Keyframe[] 
    {
" + curve.keys.ToCodeString("        ") +
    @"
    }
};";
        }

        #endregion Animation Curves

    }

}

