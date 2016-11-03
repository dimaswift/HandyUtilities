using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace HandyUtilities
{
    public interface ICustomEditorIcon
    {
        Texture2D editorIcon { get; }
        float editorIconSize { get; }
    }

    public interface ICustomEditorIconDrawer
    {
        void DrawEditorIcon(Rect rect);
    }

    public class RigidbodyPositionSaver : PositionSaver
    {
        Rigidbody m_body;

        public Rigidbody body { get { return m_body; } }

        public RigidbodyPositionSaver(Rigidbody body) : base(body.transform)
        {
            m_body = body;
        }

        public override void Restore()
        {
            m_body.isKinematic = true;
            base.Restore();
            m_body.isKinematic = false;
        }
    }

    public class Rigidbody2DPositionSaver : PositionSaver
    {
        Rigidbody2D m_body;

        public Rigidbody2D body { get { return m_body; } }

        public Rigidbody2DPositionSaver(Rigidbody2D body) : base(body.transform)
        {
            m_body = body;
        }

        public override void Restore()
        {
            m_body.isKinematic = true;
            base.Restore();
            m_body.isKinematic = false;
        }
    }

    public class PositionSaver
    {
        Vector3 m_localEuler, m_localPos, m_localScale;
        Transform m_transform, m_parent;

        public Vector3 localEuler { get { return m_localEuler; } }
        public Vector3 localPos { get { return m_localPos; } }
        public Vector3 localScale { get { return m_localScale; } }

        public Transform transform { get { return m_transform; } }
        public Transform parent { get { return m_parent; } }

        public PositionSaver(Transform transform)
        {
            this.m_transform = transform;
            this.m_parent = transform.parent;
            Save();
        }

        public virtual void Save()
        {
            m_localScale = m_transform.localScale;
            m_localPos = m_transform.localPosition;
            m_localEuler = m_transform.localEulerAngles;
        }

        public virtual void Restore()
        {
            m_transform.SetParent(m_parent);
            m_transform.localScale = m_localScale;
            m_transform.localPosition = m_localPos;
            m_transform.localEulerAngles = m_localEuler;
        }
    }

    [System.Serializable]
    public class CurvedAnimation
    {
        [SerializeField]
        AnimationCurve m_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        float m_speed = 10f;
        [SerializeField]
        bool m_loop = false;

        float m_time;
        bool m_isAnimating, m_inited;
        Keyframe m_lastFrame, m_firstFrame;

        public Keyframe firstFrame { get { return m_firstFrame; } }

        public float time { get { return m_time; } }

        public Keyframe lastFrame { get { return m_lastFrame; } }

        public bool isAnimating { get { return m_isAnimating; } }

        public CurvedAnimation() { }

        public CurvedAnimation(AnimationCurve curve, float speed, bool loop = false)
        {
            this.m_curve = curve;
            this.m_speed = speed;
            this.m_loop = loop;
            Init();
        }

        public void Init()
        {
            m_lastFrame = m_curve.keys.LastItem();
            m_firstFrame = m_curve.keys[0];
            m_inited = true;
        }

        public void Stop()
        {
            m_isAnimating = false;
            m_time = m_firstFrame.time;
        }

        public void Pause()
        {
            m_isAnimating = false;
        }

        public void Resume()
        {
            m_isAnimating = true;
        }

        public void Start()
        {
            m_isAnimating = true;
            m_time = m_firstFrame.time;
            if (!m_inited)
                Init();
        }

        public float Evaluate(float delta)
        {
            if (!m_inited)
                Init();
            float v = 0f;
            if (m_time <= m_lastFrame.time)
            {
                v = m_curve.Evaluate(m_time);
                m_time += delta * m_speed;
            }
            else
            {
                if (m_loop)
                    Start();
                else m_isAnimating = false;
                v = m_lastFrame.value;
            }
            return v;
        }
    }

    [System.Serializable]
    public sealed class Grid
    {
        [SerializeField]
        float[] _rows = new float[16];

        [SerializeField]
        float[] _columns = new float[16];

        [SerializeField]
        float _size = 1f;

        public float size { get { return _size; } }

        public float columnCount { get { return _columns.Length; } }

        public float rowCount { get { return _rows.Length; } }

        public Grid(int columns, int rows, float size, Vector2 origin)
        {
            _rows = new float[rows];
            _columns = new float[columns];
            Recalculate(columns, rows, size, origin);
        }

        public void Recalculate(int columns, int rows, float size, Vector2 origin)
        {
            columns = Mathf.Clamp(columns, 3, 512);
            rows = Mathf.Clamp(rows, 3, 512);
            _size = size;
            if (columns != _columns.Length)
                System.Array.Resize(ref _columns, columns);
            if (rows != _rows.Length)
                System.Array.Resize(ref _rows, rows);
            for (int i = 0; i < _rows.Length; i++)
            {
                _rows[i] = origin.y + i * size;
            }
            for (int i = 0; i < _columns.Length; i++)
            {
                _columns[i] = origin.x + i * size;
            }
        }

        public float GetRow(int i)
        {
            if (i < _rows.Length)
                return _rows[i];
            else return 0;
        }

        public float GetColumn(int i)
        {
            if (i < _columns.Length)
                return _columns[i];
            else return 0;
        }

        public float GetClosetRow(float row)
        {
            row -= size * .5f;
            var xDist = float.MaxValue;
            int closestX = -1;
            for (int x = 0; x < _rows.Length; x++)
            {
                var xd = Mathf.Abs(_rows[x] - row);
                if (xd < xDist)
                {
                    closestX = x;
                    xDist = xd;
                }
            }
            return _rows[closestX];
        }

        public float GetClosetColumn(float col)
        {
            col -= size * .5f;
            var yDist = float.MaxValue;
            int closestY = -1;
            for (int y = 0; y < _columns.Length; y++)
            {
                var xd = Mathf.Abs(_columns[y] - col);
                if (xd < yDist)
                {
                    closestY = y;
                    yDist = xd;
                }
            }
            return _columns[closestY];
        }
        public Vector2 GetPoint(Vector2 point)
        {

            var pos = new Vector2(GetClosetColumn(point.x), GetClosetRow(point.y));
            pos.x += size * .5f;
            pos.y += size * .5f;
            return pos;
        }
    }

    public sealed class Timer
    {
        float _rate;
        float _t;
        bool _loop;
        bool _expired;

        public float currentTime { get { return _t; } }

        public float currentTimeNormalized { get { return _t / _rate; } }

        public float currentTimeLeft { get { return _rate - _t; } }

        public bool IsExpired { get { return _expired; } }

        public bool IsRunning { get { return _t != 0 && _t < _rate; } }

        public bool IsHalfWayThere { get { return _t >= _rate * .5f; } }

        public float rate { get { return _rate; } }

        public override string ToString()
        {
            return string.Format("{0:0.0}", _rate - _t);
        }

        public void Reset()
        {
            _t = 0;
            _expired = false;
        }

        public void SetRate(float rate)
        {
            this._rate = rate;
        }

        public Timer(float rate)
        {
            this._rate = rate;
            _loop = true;
            this._expired = false;
            _t = 0;
        }

        public Timer(float rate, bool loop) : this(rate)
        {
            this._loop = loop;
        }

        public void SetReady()
        {
            _t = _rate;
        }

        public bool Wait()
        {
            if (_expired && !_loop) return false;
            _t += Time.deltaTime;
            if (_t >= _rate)
            {
                _t = 0;
                _expired = true;
                return true;
            }
            return false;
        }

    }

  
    [System.Serializable]
    public struct RandomFloatRange
    {
        [SerializeField]
        float m_min, m_max;

        public float min { get { return m_min; } set { m_min = value; } }
        public float max { get { return m_max; } set { m_max = value; } }

        public RandomFloatRange(float min, float max)
        {
            m_min = min;
            m_max = max;
        }

        public float Get()
        {
            return UnityEngine.Random.Range(min, max);
        }

        public void SetMin(float min)
        {
            this.m_min = min;
        }

        public void SetMax(float max)
        {
            this.m_max = max;
        }
        public void Increase(float delta)
        {
            m_min += delta;
            m_max += delta;
        }
    }

    [System.Serializable]
    public struct RandomIntRange
    {
        [SerializeField]
        int m_min, m_max;

        public int min { get { return m_min; } set { m_min = value; } }
        public int max { get { return m_max; } set { m_max = value; } }

        public RandomIntRange(int min, int max)
        {
            m_min = min;
            m_max = max;
        }

        public int Get()
        {
            return UnityEngine.Random.Range(min, m_max);
        }

        public void SetMin(int min)
        {
            this.min = min;
        }

        public void SetMax(int max)
        {
            this.m_max = max;
        }

        public void Increase(int delta)
        {
            min += delta;
            m_max += delta;
        }
    }

    public static class Helper
    {
        #region Properties

        public static bool isMouseWheelScrolling { get { return Input.GetAxis("Mouse ScrollWheel") != 0; } }
        public static float mouseWheelDelta { get { return Input.GetAxis("Mouse ScrollWheel"); } }
        public static bool isPointerOverGUI { get { return EventSystem.current.IsPointerOverGameObject(); } }

        #endregion

        #region Fields

        static Camera m_mainCam;
        static Collider2D[] m_collidersArray = new Collider2D[10];
        static Transform m_mainCamTransform;
        static RaycastHit[] m_raycastCache = new RaycastHit[10];
        static Plane plane = new Plane(Vector3.forward, Vector3.zero);

        #endregion

        #region Initilization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            m_mainCam = Camera.main;
            m_mainCamTransform = m_mainCam ? m_mainCam.transform : null;
        }

        #endregion Initilization

        #region Static Methods 

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                if(file.Extension != "meta")
                {
                    string temppath = System.IO.Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static System.Type GetType(string TypeName)
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = System.Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {

                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;

                // Ask that assembly to return the proper Type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;

            }

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {

                // Load the referenced assembly
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    // See if that assembly defines the named type
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }

            // The type just couldn't be found...
            return null;

        }
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float Remap(float value, float from, float to)
        {
            return (value * (to - from)) + from;
        }

        public static void PauseEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }

        public static string EncodeToCyrylic(string text)
        {
            System.Text.UTF8Encoding encodingUnicode = new System.Text.UTF8Encoding();
            byte[] cyrillicTextByte = encodingUnicode.GetBytes(text);
            return encodingUnicode.GetString(cyrillicTextByte);
        }

        public static string ConvertLoRelativePath(string absolutepath)
        {
            return "Assets" + absolutepath.Substring(Application.dataPath.Length);
        }

        public static string ConvertToAbsolutePath(string relativePath)
        {
            return Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + relativePath;
        }

        public static void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static Color RandomColor(float a = 1f)
        {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), a);
        }

        public static Vector3 GetMouse(float z = 0f)
        {
            if (!m_mainCam.orthographic)
            {
                Ray ray = m_mainCam.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.forward, Vector3.zero);
                float d = 0f;
                plane.Raycast(ray, out d);
                var pos = ray.GetPoint(d);
                pos.z = z;
                return pos;
            }
            var m = m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            m.z = z;
            return m;
        }

        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        public static float To360Angle(float angle)
        {
            while (angle < 0.0f) angle += 360.0f;
            while (angle >= 360.0f) angle -= 360.0f;
            return angle;
        }

        public static float To90Angle(float angle)
        {
            while (angle < 0.0f) angle += 90.0f;
            while (angle >= 90.0f) angle -= 90.0f;
            return angle;
        }

        public static float To180Angle(float angle)
        {
            while (angle < -180.0f) angle += 360.0f;
            while (angle >= 180.0f) angle -= 360.0f;
            return angle;
        }

        public static float ToAngle(float angle, float offset)
        {
            if (offset == 0) return angle;
            while (angle < -offset) angle += offset;
            while (angle >= offset) angle -= offset;
            return angle;
        }

        public static float CathetusLenght(float hypotenuse, float cathetus)
        {
            return (float) System.Math.Sqrt((hypotenuse * hypotenuse) - (cathetus * cathetus));
        }

        public static List<T> LoadResourcesOfType<T>(string path)
        {
            var objs = Resources.LoadAll<GameObject>(path);
            var list = new List<T>();
            foreach (var o in objs)
            {
                if (o.GetComponent<T>() != null)
                    list.Add(o.GetComponent<T>());
            }
            return list;
        }

        public static string SecondsTo_hh_mm_ss(int seconds)
        {
            if (seconds >= 3600)
                return string.Format("{0:00}:{1:00}:{2:00}", seconds / 3600, (seconds / 60) % 60, seconds % 60);
            if (seconds >= 60)
                return string.Format("{0:00}:{1:00}", (seconds / 60) % 60, seconds % 60);
            return string.Format("0:{0:00}", seconds);
        }

        public static float EulerToTarget(Vector2 origin, Vector2 target, bool normalize = false)
        {
            target.x = target.x - origin.x;
            target.y = target.y - origin.y;
            var result = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            if (float.IsNaN(result))
                return 0;
            return normalize ? To180Angle(result) : result;
        }

        public static float EulerToMouse(Vector2 origin, bool normalize = false)
        {
            var target = GetMouse();
            target.x = target.x - origin.x;
            target.y = target.y - origin.y;
            var result = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            return normalize ? To180Angle(result) : result;
        }

        public static string NormalizeFloat(float target)
        {
            if (target >= 1000000)
            {
                target /= 1000000;
                return target.ToString("0.0M");
            }
            if (target >= 1000)
            {
                target /= 1000;
                return target.ToString("0.0K");
            }
            if (target == 0)
            {
                return string.Empty;
            }
            return target.ToString("0");
        }

        public static float[] KeepSum(float sum, float[] edited, float[] original)
        {
            for (int i = 0; i < original.Length; i++)
            {
                if (edited[i] != original[i])
                {
                    var delta = edited[i] - original[i];
                    original[i] += delta;
                    delta /= edited.Length - 1;
                    int trycount = 10;
                    for (int j = 0; j < original.Length; j++)
                    {
                        if (j != i)
                        {
                            if (original[j] >= delta)
                            {
                                original[j] -= delta;
                            }
                            else
                            {
                                delta += original[j];
                            }
                        }
                    }
                    delta = Mathf.Abs(sum - FindSum(original));
                    while (delta > 0.0001f && trycount > 0)
                    {
                        var left = System.Array.FindAll(original, (n) => { return n != original[i] && n > 0; });
                        delta /= left.Length + 1;
                        for (int l = 0; l < left.Length; l++)
                        {
                            var ind = System.Array.FindIndex(original, (n) => { return n == left[l]; });
                            if (original[ind] >= delta)
                            {
                                original[ind] -= delta;
                            }
                            else
                            {
                                delta += original[ind];
                            }

                        }
                        delta = Mathf.Abs(sum - FindSum(original));
                        trycount--;

                    }
                    break;
                }
            }
            return original;
        }

        public static float FindSum(float[] numbers)
        {
            float res = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                res += numbers[i];
            }
            return res;
        }

        public static int FindSum(int[] numbers)
        {
            int res = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                res += numbers[i];
            }
            return res;
        }

        public static float FindSum(List<float> numbers)
        {
            float res = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                res += numbers[i];
            }
            return res;
        }

        public static int FindSum(List<int> numbers)
        {
            int res = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                res += numbers[i];
            }
            return res;
        }

        public static bool HasType(GameObject target, System.Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                if (target.GetComponent(t) != null || target.GetComponentInParent(t) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static float DistanceToLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }



        public static bool IsOverlappingPoint(Vector2 position)
        {
            return Physics2D.OverlapPointNonAlloc(position, m_collidersArray) > 0;
        }

        public static bool IsOverlappingPoint(Vector2 position, int layer)
        {
            return Physics2D.OverlapPointNonAlloc(position, m_collidersArray, 1 << layer) > 0;
        }

        public static bool IsOverlappingCircle(Vector2 position, float radius)
        {
            return Physics2D.OverlapCircleNonAlloc(position, radius, m_collidersArray) > 0;
        }

        public static bool IsOverlappingCircle(Vector2 position, float radius, int layer)
        {
            return Physics2D.OverlapCircleNonAlloc(position, radius, m_collidersArray, 1 << layer) > 0;
        }

        public static Collider2D[] OverlapPoint(Vector2 position, out int count)
        {
            count = Physics2D.OverlapPointNonAlloc(position, m_collidersArray);
            return m_collidersArray;
        }

        public static Collider2D[] OverlapPoint(Vector2 position, int layer, out int count)
        {
            count = Physics2D.OverlapPointNonAlloc(position, m_collidersArray, 1 << layer);
            return m_collidersArray;
        }

        public static Collider2D[] OverlapCircle(Vector2 position, float radius, out int count)
        {
            count = Physics2D.OverlapCircleNonAlloc(position, radius, m_collidersArray);
            return m_collidersArray;
        }

        public static Collider2D[] OverlapCircle(Vector2 position, float radius, int layer, out int count)
        {
            count = Physics2D.OverlapCircleNonAlloc(position, radius, m_collidersArray, 1 << layer);
            return m_collidersArray;
        }


        public static RaycastHit[] raycastCache { get { return m_raycastCache; } }

        public static Vector3 mainCameraDirection { get { return m_mainCamTransform.TransformDirection(Vector3.forward); } }

        public static Vector3 viewport
        {
            get { return mainCamera.ScreenToViewportPoint(Input.mousePosition); }
        }

        public static Camera mainCamera
        {
            get
            {
                if (m_mainCam == null)
                {
                    m_mainCam = Camera.main;
                    if (m_mainCam == null)
                        m_mainCam = Object.FindObjectOfType<Camera>();
                }
                return m_mainCam;
            }
        }

        public static int isometricMouseRaycast(int mask)
        {
            return Physics.RaycastNonAlloc(mousePosition, mainCameraDirection, m_raycastCache, mask);
        }

        public static RaycastHit Raycast(Ray ray)
        {
            int hits = Physics.RaycastNonAlloc(ray, m_raycastCache);
            RaycastHit hit = new RaycastHit() { distance = float.PositiveInfinity };
            for (int i = 0; i < hits; i++)
            {
                if (hit.distance > raycastCache[i].distance)
                    hit = raycastCache[i];
            }
            return hit;
        }

        public static RaycastHit RaycastFromScreenCenter()
        {
            return Raycast(GetCenterScreenRay());
        }

        public static RaycastHit RaycastMouse()
        {
            int hits = Physics.RaycastNonAlloc(m_mainCam.ScreenPointToRay(Input.mousePosition), m_raycastCache);
            RaycastHit hit = new RaycastHit() { distance = float.PositiveInfinity };
            for (int i = 0; i < hits; i++)
            {
                if (hit.distance > raycastCache[i].distance)
                    hit = raycastCache[i];
            }
            return hit;
        }



        public static bool IsTouchPhase(TouchPhase phase)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == phase)
                {
                    return true;
                }
            }
            return false;
        }

        public static int RaycastAll(Ray ray)
        {
            return Physics.RaycastNonAlloc(ray, m_raycastCache);
        }

        public static Ray ProjectIsometricRay()
        {
            float d = 0f;
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out d);
            return ray;
        }

        public static Ray GetCenterScreenRay()
        {
            return mainCamera.ScreenPointToRay(new Vector2(Screen.width * .5f, Screen.height * .5f));
        }

        public static Ray ProjectFPSRay()
        {
            return GetCenterScreenRay();
        }

        public static Vector3 ProjectIsometricRay(Ray ray, float height = 0f)
        {
            float d = 0f;
            plane.Raycast(ray, out d);
            return ray.GetPoint(d - height);
        }

        public static int IsometricMouseRaycast()
        {
            return Physics.RaycastNonAlloc(mousePosition, mainCameraDirection, m_raycastCache);
        }

        public static Vector3 mousePosition
        {
            get { return mainCamera.ScreenToWorldPoint(Input.mousePosition); }
        }


        #endregion

    }
}
