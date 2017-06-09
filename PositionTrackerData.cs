using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    [CreateAssetMenu(menuName = "Handy Utilities/Position Tracker Data")]
    public class PositionTrackerData : ScriptableObject
    {
        public int maxCount = 1000;
        public GameObject gizmo;
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
            if (positions.Count > maxCount)
                return;
            positions.Add(target.position);
            eulers.Add(target.eulerAngles);
        }
    }

}
