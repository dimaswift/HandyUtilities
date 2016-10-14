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
    public Drag.DragType _dragType = Drag.DragType.Rigidbody;
    public Drag.HookType _hookType = Drag.HookType.ByCollider;
    public GameObject _dragTarget { get; set; }
    public SpringJoint2D spring { get; set; }
    public Rigidbody2D springBody { get; set; }
    public LineRenderer line { get; set; }
    public bool useLine;
    public RigidbodyConstraints2D _dragConstraints = RigidbodyConstraints2D.None;
    public bool canSlice
    {
        get { return Drag.canSlice; }
        set  { Drag.canSlice = value;  }
    }
    public void Awake()
    {
        Drag.canSlice = true;
        Drag.manager = GetComponent<Dragger2D>();
        springBody = gameObject.GetComponent<Rigidbody2D>();
        spring = gameObject.GetComponent<SpringJoint2D>();
        line = gameObject.GetComponent<LineRenderer>();
        if (line == null)
            useLine = false;
        if(useLine)
        {
            line.useWorldSpace = false;
            line.SetVertexCount(2);
        }
          
        spring.enabled = false;
        springBody.isKinematic = true;

    }
    void FixedUpdate()
    {
        Drag.PhysicsUpdate();
    }
    void Update()
    {
        Drag.WaitForHook();
    }
}

public sealed class Drag
{
    public enum HookType { ByCollider, ByDistance }
    public enum DragType { Rigidbody, Transform }
    public static bool canSlice { get; set; }
    // Static Fields

    static Vector2 hookPoint;
    static Rigidbody2D draggedBody;
    static Vector3 pressedMouse, pressedTarget;
    static Transform draggedTransform;
    static bool dragging;
    static Dragger2D _manager;
    static RigidbodyConstraints2D lastContraints;

    public static Dragger2D manager
    {
        get
        {
            if (_manager == null)
            {
                var g = new GameObject("DragManager");
                Debug.LogWarning("Accessing null manager. \nNew drag manager created.");
                _manager = g.AddComponent<Dragger2D>();
                _manager.Awake();
            }
            return _manager;
        }
        set
        {
            if(_manager == null)
            {
                _manager = value;
            }
        }
    }

    // Properties' fields


    // Public Properties
    /// <summary>
    /// Mouse button index used to hook objects.
    /// </summary>
    public static int mouseButton
    {
        get
        {
            return manager._mouseButton;
        }
        set
        {
            manager._mouseButton = value;
        }
    }

    /// <summary>
    /// If true, checks all targets available and hooks closest one.
    /// </summary>
    public static bool dragAll
    {
        get
        {
            return manager._dragAll;
        }
        set
        {
            manager._dragAll = value;
        }
    }
    /// <summary>
    /// Layers that can be dragged.
    /// </summary>
    public static LayerMask layerMask
    {
        get
        {
            return manager._layerMask;
        }
        set
        {
            manager._layerMask = value;
        }
    }
    /// <summary>
    /// Dragging body constraints.
    /// </summary>
    public static RigidbodyConstraints2D dragConstraints
    {
        get
        {
            return manager._dragConstraints;
        }
        set
        {
            manager._dragConstraints = value;
        }
    }
    /// <summary>
    /// Force of drag.
    /// </summary>
    public static float dragForce
    {
        get
        {
            return manager._dragForce;
        }
        set
        {
            manager._dragForce = value;
        }
    }
    /// <summary>
    /// Freeze Z rotation of dragging rigidbody?
    /// </summary>
    public static bool freezeRotation
    {
        get
        {
            return manager._freezeRotation;
        }
        set
        {
            manager._freezeRotation = value;
        }
    }
    /// <summary>
    /// Minimal distance between mouse and target when using ByDistance hook.
    /// </summary>
    public static float hookDistance
    {
        get
        {
            return manager._hookDistance;
        }
        set
        {
            manager._hookDistance = value;
        }
    }

    /// <summary>
    /// Switches between drag type. SpringHook - drags rigid body using Spring Joint 2D, BodyDrop - simply applies force to the body, and Transform - moves transform with offset.
    /// </summary>
    public static DragType dragType
    {
        get
        {
            return manager._dragType;
        }
        set
        {
            manager._dragType = value;
        }
    }

    /// <summary>
    /// Switches between hook method. ByCollider - performs Physics2D overlap check. ByDistance - uses hookDistance value to check distance betewwn mouse and target before hooking.
    /// </summary>
    public static HookType hookType
    {
        get
        {
            return manager._hookType;
        }
        set
        {
            manager._hookType = value;
        }
    }

    /// <summary>
    /// Current target of drag.
    /// </summary>
    public static GameObject dragTarget
    {
        get
        {
            return manager._dragTarget;
        }
        set
        {
            manager._dragTarget = value;
        }
    }

    /// <summary>
    /// Max distance of spring joint.
    /// </summary>
    public static float springDistance
    {
        get
        {
            return manager._springDistance;
        }
        set
        {
            manager._springDistance = value;
        }
    }

    /// <summary>
    /// Whether or not force is multiplied by target body mass.
    /// </summary>
    public static bool multiplyByMass
    {
        get
        {
            return manager._multiplyByMass;
        }
        set
        {
            manager._multiplyByMass = value;
        }
    }

    /// <summary>
    /// Is drag being performed?
    /// </summary>
    public static bool isDragging
    {
        get
        {
            return dragging;
        }
    }

    /// <summary>
    /// Makes rigidbody static while dragging.
    /// </summary>
    public static bool makeStatic
    {
        get
        {
            return manager._makeStatic; 
        }
        set
        {
            manager._makeStatic = value;
        }
    }


    /// <summary>
    /// Initilizes drag creating spring joint body.
    /// </summary>

    /// <summary>
    /// Put this in single FixedUpdate function. Functon when physical drag is performed.
    /// </summary>
    public static void PhysicsUpdate()
    {
        if (dragging)
        {
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
           
            var pos = mouse;
            pos.x = dragConstraints != RigidbodyConstraints2D.FreezePositionX ? pressedTarget.x + (mouse.x - pressedMouse.x) : draggedTransform.position.x;
            pos.y = dragConstraints != RigidbodyConstraints2D.FreezePositionY ? pressedTarget.y + (mouse.y - pressedMouse.y) : draggedTransform.position.y;
            pos.z = pressedTarget.z;
            var lerp = Vector3.Lerp(draggedTransform.position, pos, manager._smoothDamp);
      

            if (draggedBody != null)
            {
                if (manager._freezeRotation)
                    draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                else
                    draggedBody.constraints = manager._dragConstraints;
                switch (dragType)
                {
                    case DragType.Rigidbody:
                        if (makeStatic)
                        {
                            draggedBody.velocity = Vector2.zero;
                            draggedBody.angularVelocity = 0;
                            draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                            draggedBody.MovePosition(lerp);
                        }
                        else
                        {
                            if(freezeRotation)
                                draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                            manager.springBody.MovePosition(mouse);
                            manager.springBody.mass =  dragForce * 50;
                            var f = 1.1f - (springDistance + 0.01f);
                            if (multiplyByMass)
                            {
                                f *= manager.springBody.mass;
                            }
                            manager.spring.frequency = f;
                            
                        }
                        break;
                    case DragType.Transform:
                        draggedBody.constraints = draggedBody.constraints | RigidbodyConstraints2D.FreezeRotation;
                        draggedBody.MovePosition(lerp);
                        break;
                }
            }
            else if(dragType == DragType.Transform)
            {
                draggedTransform.position = lerp;
            }
         
        }
    }

    /// <summary>
    /// Input method. Put it in Update function.
    /// </summary>
    public static void WaitForHook()
    {
        if (Input.GetMouseButtonUp(mouseButton))
        {
            EndDrag();
            return;
        }
        if (dragging)
        {
        //    manager.transform.LookAtEuler(draggedTransform.transform.TransformPoint(hookPoint), Space.World);
            if(manager.useLine)
            {
                manager.line.SetPosition(0, Vector3.zero);
                manager.line.SetPosition(1, manager.transform.InverseTransformPoint(draggedTransform.transform.TransformPoint(hookPoint)));
                var w = 0.5f - (Vector2.Distance(manager.transform.InverseTransformPoint(draggedTransform.transform.TransformPoint(hookPoint)), Vector2.zero) / 50);
                w = Mathf.Clamp(w, 0.01f, 0.1f);
                manager.line.SetWidth(w, w);
            }
        
        }
        if (Input.GetMouseButtonDown(mouseButton))
        {
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (hookType == HookType.ByCollider)
            {
                var overlap = Physics2D.OverlapPointAll(mouse, layerMask);
                foreach (var o in overlap)
                {
                    bool readyToDrag = dragAll ? true : dragTarget == o.gameObject;
                    if (readyToDrag)
                    {
                        StartDrag(o.gameObject, mouse);
                        break;
                    }
                }
            }
            else if(dragTarget != null)
            {
                var d = Vector3.Distance(mouse, dragTarget.transform.position);
                if (d <= hookDistance)
                {
                    StartDrag(dragTarget, mouse);
                }
            }
        }
    }
    static void EndDrag()
    {
        dragging = false;
        if(manager.useLine)
            manager.line.enabled = false;
        manager.spring.enabled = false;
        manager.springBody.isKinematic = true;
        if(draggedBody)
         draggedBody.constraints = lastContraints;
        draggedBody = null;
    }
    static void StartDrag(GameObject target, Vector3 point)
    {
        hookPoint = target.transform.InverseTransformPoint(point);
        pressedMouse = point;
        draggedBody = target.GetComponent<Rigidbody2D>();
        draggedTransform = target.transform;
        pressedTarget = target.transform.position;

        if (draggedBody != null)
        {
            if (manager.useLine)
                manager.line.enabled = true;
            if (!makeStatic)
            {
                manager.springBody.transform.position = point;
                manager.spring.enabled = true;
                manager.springBody.isKinematic = false;
                manager.spring.connectedBody = draggedBody;
                manager.spring.connectedAnchor = hookPoint;
                
            }
            lastContraints = draggedBody.constraints;
        }
        dragging = true;
    }

}
