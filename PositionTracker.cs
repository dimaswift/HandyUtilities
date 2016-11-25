using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public class PositionTracker : MonoBehaviour
    {
        public float distanceRate = .5f;

        Vector3 m_prevPosition;

        [SerializeField]
        PositionTrackerData m_data;

        [SerializeField]
        bool m_recordOnStart = false;

        [SerializeField]
        KeyCode m_startRecordingKey = KeyCode.R;

        [SerializeField]
        KeyCode m_endRecordingKey = KeyCode.S;

        bool m_isRecording;

        void Start()
        {
            if (m_recordOnStart)
                StartRecording();
        }

        void StartRecording()
        {
            m_isRecording = true;
            m_data.Clear();
            m_data.startPoint = transform.position;
        }

        void Update()
        {
            if(Input.GetKeyDown(m_startRecordingKey))
            {
                StartRecording();
            }
            if (Input.GetKeyDown(m_endRecordingKey))
            {
                m_isRecording = false;
            }
            if (m_isRecording)
            {
                var d = Vector3.Distance(transform.position, m_prevPosition);
                if (d > distanceRate)
                {
                    m_prevPosition = transform.position;
                    m_data.AddSnapshot(transform);
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(m_data);
#endif
                }
            }
        }
    }

}
