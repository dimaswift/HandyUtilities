using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(SpringJoint2D))]
public class Dragger2D : MonoBehaviour
{
    [Range(0, 3)]
    public int _mouseButton = 0;
    public bool _dragAll = true, _multiplyByMass, _freezeRotation, _makeStatic;
    public LayerMask _layerMask = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7;
    [Range(0f, 1f)]
    public float _dragForce = .5f, _hookDistance = .5f, _springDistance = .5f, _smoothDamp;
    public DragType _dragType = DragType.Rigidbody;
    public HookType _hookType = HookType.ByCollider;
    public GameObject _dragTarget { get; set; }
    public SpringJoint2D spring { get; set; }
    public Rigidbody2D springBody { get; set; }
    public LineRenderer line { get; set; }
    public bool useLine;
    public RigidbodyConstraints2D _dragConstraints = RigidbodyConstraints2D.None;

    public enum HookType { ByCollider, ByDistance }
    public enum DragType { Rigidbody, Transform }
    public static bool canSlice { get; set; }
    // Static Fields

    Vector2 hookPoint;
    Rigidbody2D draggedBody;
    Vector3 pressedMouse, pressedTarget;
    Transform draggedTransform;
    bool dragging;
    Dragger2D _manager;
    RigidbodyConstraints2D lastContraints;

    public void Awake()
    {
        canSlice = true;
        springBody = gameObject.GetComponent<Rigidbody2D>();
        spring = gameObject.GetComponent<SpringJoint2D>();
        spring.enabled = false;
        springBody.isKinematic = true;
    }

    void Update()
    {
        WaitForHook();
    }

    void FixedUpdate()
    {
        if (dragging)
        {
            var mouse = GetMousePos();

            var pos = mouse;
            pos.x = _dragConstraints != RigidbodyConstraints2D.FreezePositionX ? pressedTarget.x + (mouse.x - pressedMouse.x) : draggedTransform.position.x;
            pos.y = _dragConstraints != RigidbodyConstraints2D.FreezePositionY ? pressedTarget.y + (mouse.y - pressedMouse.y) : draggedTransform.position.y;
            pos.z = pressedTarget.z;
            var lerp = Vector3.Lerp(draggedTransform.position, pos, _smoothDamp);


            if (draggedBody != null)
            {
                if (_freezeRotation)
                    draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                else
                    draggedBody.constraints = _dragConstraints;
                switch (_dragType)
                {
                    case DragType.Rigidbody:
                        if (_makeStatic)
                        {
                            draggedBody.velocity = Vector2.zero;
                            draggedBody.angularVelocity = 0;
                            draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                            draggedBody.MovePosition(lerp);
                        }
                        else
                        {
                            if (_freezeRotation)
                                draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                            springBody.MovePosition(mouse);
                            springBody.mass = _dragForce * 50;
                            var f = 1.1f - (_springDistance + 0.01f);
                            if (_multiplyByMass)
                            {
                                f *= springBody.mass;
                            }
                            spring.frequency = f;

                        }
                        break;
                    case DragType.Transform:
                        draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                        draggedBody.MovePosition(lerp);
                        break;
                }
            }
            else if (_dragType == DragType.Transform)
            {
                draggedTransform.position = lerp;
            }

        }
    }

    Vector3 GetMousePos()
    {
        var plane = new Plane(-Vector3.forward, Vector3.zero);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float d = 0;
        plane.Raycast(ray, out d);
        return ray.GetPoint(d);
    }

    public void WaitForHook()
    {
        if (Input.GetMouseButtonUp(_mouseButton))
        {
            EndDrag();
            return;
        }

        if (Input.GetMouseButtonDown(_mouseButton))
        {
            var mouse = GetMousePos();
            if (_hookType == HookType.ByCollider)
            {
                var overlap = Physics2D.OverlapPointAll(mouse, _layerMask);
                foreach (var o in overlap)
                {
                    bool readyToDrag = _dragAll ? true : _dragTarget == o.gameObject;
                    if (readyToDrag)
                    {
                        StartDrag(o.gameObject, mouse);
                        break;
                    }
                }
            }
            else if (_dragTarget != null)
            {
                var d = Vector3.Distance(mouse, _dragTarget.transform.position);
                if (d <= _hookDistance)
                {
                    StartDrag(_dragTarget, mouse);
                }
            }
        }
    }

    void EndDrag()
    {
        dragging = false;
        spring.enabled = false;
        springBody.isKinematic = true;
        if (draggedBody)
            draggedBody.constraints = lastContraints;
        draggedBody = null;
    }

    void StartDrag(GameObject target, Vector3 point)
    {
        hookPoint = target.transform.InverseTransformPoint(point);
        pressedMouse = point;
        draggedBody = target.GetComponent<Rigidbody2D>();
        draggedTransform = target.transform;
        pressedTarget = target.transform.position;

        if (draggedBody != null)
        {
            if (!_makeStatic)
            {
                springBody.transform.position = point;
                spring.enabled = true;
                springBody.isKinematic = false;
                spring.connectedBody = draggedBody;
                spring.connectedAnchor = hookPoint;
            }
            lastContraints = draggedBody.constraints;
        }
        dragging = true;
    }
}



