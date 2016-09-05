using UnityEngine;
using System.Collections;

namespace HandyUtilities
{
    public class CameraObserver : MonoBehaviour
    {
        [SerializeField]
        Vector3 target;
        [SerializeField]
        Transform targetTransform;
        [SerializeField]
        float rotateSpeed = 500;
        [SerializeField]
        float zoomSpeed = 500;
        [SerializeField]
        float dragSpeed = 500;
 
        [Range(0f, 1f)]
        [SerializeField]
        float smoothTime = .5f;




        Transform _transform;
        bool _useTransformTarget;
        Vector3 _currentRotatePos;

        Vector2 _lookAroundAxis;
        Vector2 _rotateAroundPivotAxis;
        Vector3 _moveAxis;
        float _zDepth;
        Vector3 _lerpedTargetPos;
        Vector3 _currentPivot = Vector3.zero;

        void Start()
        {
            Init();

        }


        public void Init()
        {
            _transform = transform;
            _useTransformTarget = targetTransform != null;
        }

        Vector3 GetTargetPos()
        {
            return _useTransformTarget ? targetTransform.position : target;
        }

        void MoveTo(Vector3 point)
        {
            _transform.position = Vector3.Lerp(_transform.position, point, Time.deltaTime * smoothTime * 10);
        }

        void LateUpdate()
        {
            Vector2 axis = Vector2.zero;
            axis.x = Input.GetAxis("Mouse X");
            axis.y = Input.GetAxis("Mouse Y");
            var smooth = Time.deltaTime * smoothTime * 10;

            // Taking input

            if (Input.GetMouseButton(1))
            {
                _lookAroundAxis = Vector2.Lerp(_lookAroundAxis, axis * rotateSpeed, smooth);
            }
            else _lookAroundAxis = Vector2.Lerp(_lookAroundAxis, Vector2.zero, smooth);
        
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
            {
                _rotateAroundPivotAxis = Vector2.Lerp(_rotateAroundPivotAxis, axis * rotateSpeed, smooth);
            }
            else _rotateAroundPivotAxis = Vector2.Lerp(_rotateAroundPivotAxis, Vector2.zero, smooth);

            if (Input.GetMouseButton(2))
            {
                _moveAxis = Vector2.Lerp(_moveAxis, axis, smooth);
            }
            else _moveAxis = Vector2.Lerp(_moveAxis, Vector2.zero, smooth);

            _zDepth = Mathf.Lerp(_zDepth, -Helper.mouseWheelDelta * zoomSpeed * 100, smooth);

            _moveAxis.z = _zDepth;

            // Applying axises if needed

            if (_lookAroundAxis.x != 0 || _lookAroundAxis.y != 0)
            {
                var rot = _transform.eulerAngles;
                rot.x -= _lookAroundAxis.y;
                rot.y += _lookAroundAxis.x;
                _transform.eulerAngles = rot;
            }

            if(_moveAxis.x != 0 || _moveAxis.y != 0)
            {
                _transform.Translate(-((_moveAxis) * dragSpeed) * smooth);
            }

            if (_rotateAroundPivotAxis.x != 0 || _rotateAroundPivotAxis.y != 0)
            {
                _transform.RotateAroundPivot(_currentPivot,transform.TransformDirection(_rotateAroundPivotAxis.y, _rotateAroundPivotAxis.x, 0) );
            }

        }
    }

}
