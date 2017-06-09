using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    float m_scale = 1f;

    float m_delta;

    void Awake()
    {
        m_delta = Time.fixedDeltaTime;
    }

    void Update()
    {
        if(m_scale != Time.timeScale)
        {
            Time.timeScale = m_scale;
            Time.fixedDeltaTime = m_delta * m_scale;
        }
    }
}
