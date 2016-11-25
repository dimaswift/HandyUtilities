using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    [CreateAssetMenu(menuName = "Handy Utilities/Position Tracker Data")]
    public class PositionTrackerData : ScriptableObject
    {
        public float gizmoSize = .1f;
        public PrimitiveType gizmoType = PrimitiveType.Sphere;
        [HideInInspector]
        public List<Vector3> positions;
        [HideInInspector]
        public List<Vector3> eulers;
        [HideInInspector]
        public Vector3 startPoint;

        public void StartRecording(Transform target)
        {
            startPoint = target.position;
        }

        public void Clear()
        {
            positions.Clear();
            eulers.Clear();
        }

        public void AddSnapshot(Transform target)
        {
            positions.Add(target.position);
            eulers.Add(target.eulerAngles);
        }
    }

}
