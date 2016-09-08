using UnityEngine;
using System;
namespace HandyUtilities
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement2D : BaseBehaviour
    {
        public MoveMode mode;
        [Range(0, 2)]
        public int mouseDragButton = 2;
        public float zoomSpeed = 5;
        public float scrollSpeed = 5;
        public float maxSize = 50;
        public float minSize = -50;
        public bool scrollZoomIntoMouse = true;
        public bool lerpPosition = true;
        [Range(0f, 1f)]
        public float movementSmooth = 0f;
        public bool useDragInertia;
        public Vector3 targetFollowingOffset = Vector2.zero;
        Plane m_plane;

        public enum MoveMode
        {
            MouseDrag,
            FollowTarget,
            FocusOnPoint,
            LinearMovement
        }

        // Props
        public bool isDragging { get { return m_dragging; } }
        public override State state { get { return m_state; } }
        public bool isZooming { get { return m_isZooming; } }
        public bool isMoving { get { return m_isMoving; } }
        public bool isScrolling { get { return m_isScrolling; } }

        // Members
        Vector3 m_pressedMouse, m_pressedCamPos, m_focusPoint, m_lastCamPos;
        bool m_dragging, m_isMoving, m_isZooming, m_isScrolling, m_hasTarget, m_orthographic;
        float m_orthographicSize, m_targetSize, m_lastCamSize, m_zPos, m_perspectiveDepth;
        Bounds m_cameraBounds;
        Camera m_camera;
        const string MOUSE_SCROLL_INPUT = "Mouse ScrollWheel";
        [SerializeField]
        Transform m_target;
        CameraState m_state;

        // Initilization

        public override void Init()
        {
            base.Init();
            m_pressedCamPos = cachedTransform.position;
            m_pressedMouse = m_pressedCamPos;
            m_camera = GetComponent<Camera>();
            m_orthographic = m_camera.orthographic;

            m_orthographicSize = m_camera.orthographicSize;
            if (m_orthographic)
                m_cameraBounds = new Bounds(cachedTransform.position, new Vector2(m_orthographicSize * m_camera.aspect * 2, m_orthographicSize * 2));
            else
            {
                var frustumHeight = 2.0f * -cachedTransform.position.z * Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                m_cameraBounds = new Bounds(cachedTransform.position, new Vector3(frustumHeight * m_camera.aspect, frustumHeight));
            }

            m_perspectiveDepth = cachedTransform.position.z;
            m_targetSize = m_orthographic ? m_orthographicSize : m_perspectiveDepth;
            m_hasTarget = m_target != null;
            m_zPos = cachedTransform.position.z;
            m_state = new CameraState();
            m_plane = new Plane(Vector3.forward, new Vector3(0, 0, 0));

        }

        // Events

        void Update()
        {
            switch (mode)
            {
                case MoveMode.MouseDrag:
                    ProcessDragging();
                    ProcessScolling();
                    break;
                case MoveMode.FollowTarget:
                    //    ProcessZooming();
                    break;
                case MoveMode.FocusOnPoint:
                    ProcessFocusingOnPoint();
                    ProcessZooming();
                    break;
            }
        }

        void FixedUpdate()
        {
            if (mode == MoveMode.FollowTarget)
                ProcessFollowingTarget();
        }

        // Public Functions

        public override void SaveState()
        {
            m_state.Save(this);
        }

        public Vector3 GetMouse(float z = 0)
        {
            Vector3 mouse;
            if (m_orthographic)
            {
                mouse = m_camera.ScreenToWorldPoint(Input.mousePosition);
                mouse.z = z;
            }
            else
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var d = 0f;
                m_plane.Raycast(ray, out d);
                mouse = ray.GetPoint(d);
                mouse.z = z;
            }
            return mouse;
        }

        public override void LoadState(State state)
        {
            m_state = state as CameraState;
            m_state.Load(this);
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
            m_hasTarget = true;
        }

        public void SetTargetSize(float size)
        {
            m_targetSize = size;
        }

        public void SetSizeImmediately(float size)
        {
            m_targetSize = size;
            m_orthographicSize = size;
            m_cameraBounds.size = new Vector2(m_orthographicSize * m_camera.aspect * 2, m_orthographicSize * 2);
            m_camera.orthographicSize = m_orthographicSize;
        }

        public void FocusOnPoint(Vector3 point)
        {
            mode = MoveMode.FocusOnPoint;
            m_focusPoint = point;
        }

        public void FocusOnPoint(Vector3 point, float focusSize)
        {
            FocusOnPoint(point);
            m_targetSize = focusSize;
        }

        public void SetCameraPosition(Vector3 position)
        {
            if (m_lastCamPos != position)
            {
                if (m_orthographic)
                {
                    position.z = m_zPos;
                }

                m_lastCamPos = position;
                m_cameraBounds.center = m_lastCamPos;
                cachedTransform.position = m_lastCamPos;
                m_isMoving = true;
            }
            else m_isMoving = false;
        }

        // Private Methods

        float GetSmooth()
        {
            return (1.001f - movementSmooth) * 25;
        }

        // Processing

        void ProcessDragging()
        {
            if (Input.GetMouseButtonDown(mouseDragButton))
            {
                m_dragging = true;
                m_pressedMouse = GetMouse();
                m_pressedCamPos = cachedTransform.position;
            }
            if (m_dragging)
            {
                var delta = m_pressedMouse - GetMouse();
                delta.z = 0;
                if (m_orthographic)
                {
                    delta.x *= m_cameraBounds.size.x;
                    delta.y *= m_cameraBounds.size.y;
                }
                else
                {
                    delta.x *= 2;
                    delta.y *= 2;
                }
                var pos = lerpPosition ? Vector3.Lerp(cachedTransform.position, m_pressedCamPos + delta, Time.fixedDeltaTime * GetSmooth()) : m_pressedCamPos + delta;
                SetCameraPosition(pos);
            }
            if (Input.GetMouseButtonUp(mouseDragButton))
            {
                m_dragging = false;
            }
        }

        void ProcessZooming()
        {
            if (!Mathf.Approximately(m_orthographicSize, m_targetSize))
            {
                if (m_orthographic)
                {
                    m_orthographicSize = Mathf.Lerp(m_orthographicSize, m_targetSize, Time.fixedDeltaTime * zoomSpeed);
                    m_cameraBounds.size = new Vector2(m_orthographicSize * m_camera.aspect * 2, m_orthographicSize * 2);
                    m_camera.orthographicSize = m_orthographicSize;
                }
                else
                {
                    m_perspectiveDepth = Mathf.Lerp(m_perspectiveDepth, m_targetSize, Time.fixedDeltaTime * zoomSpeed);
                    var pos = cachedTransform.position;
                    pos.z = mode == MoveMode.FollowTarget ? m_perspectiveDepth + targetFollowingOffset.z : m_perspectiveDepth;
                    SetCameraPosition(pos);
                }
                m_isZooming = true;
            }
            else m_isZooming = false;
        }

        void ProcessScolling()
        {
            var axis = Input.GetAxis(MOUSE_SCROLL_INPUT);

            if (axis != 0)
            {
                var delta = axis * scrollSpeed * (m_targetSize * .1f);
                m_targetSize -= delta;
                m_targetSize = Mathf.Clamp(m_targetSize, minSize, maxSize);
                m_isScrolling = true;
            }
            else m_isScrolling = false;
            bool closeEnough = m_orthographic ? Mathf.Approximately(m_orthographicSize, m_targetSize) : Mathf.Approximately(m_perspectiveDepth, m_targetSize);
            if (!closeEnough)
            {
                m_isZooming = true;

                ProcessZooming();
                if (!m_dragging)
                {
                    if (scrollZoomIntoMouse)
                    {
                        Vector3 delta = m_cameraBounds.size;
                        delta -= m_cameraBounds.size;
                        delta.z = 0;
                        delta = Vector3.Scale(GetMouse() - new Vector3(.5f, .5f), delta);
                        SetCameraPosition(cachedTransform.position + delta);
                    }
                }
            }
            else
            {
                m_isZooming = false;
            }
        }

        void ProcessFocusingOnPoint()
        {
            var pos = lerpPosition ? Vector3.Lerp(m_lastCamPos, m_focusPoint, Time.fixedDeltaTime * GetSmooth()) : m_focusPoint;
            SetCameraPosition(pos);
        }

        void ProcessFollowingTarget()
        {
            if (m_hasTarget)
            {
                var pos = lerpPosition ? Vector3.Lerp(m_lastCamPos, m_target.position - targetFollowingOffset, Time.fixedDeltaTime * GetSmooth()) : m_target.position - targetFollowingOffset;
                SetCameraPosition(pos);
            }
        }

        [Serializable]
        public class CameraState : TransfromState
        {
            public float size;

            public void Load(CameraMovement2D cam)
            {
                cam.SetSizeImmediately(size);
                base.Load(cam);
            }

            public override void Save(BaseBehaviour behaviour)
            {
                base.Save(behaviour);
                var cam = behaviour as CameraMovement2D;
                size = cam.m_orthographicSize;
            }
        }
    }

}
