using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class StartShaking : MonoBehaviour
{
    
    [SerializeField] float m_magnitude;
    [SerializeField] float m_roughness;
    [SerializeField] float m_fadeInTime;
    [SerializeField] float m_fadeOutTime;

    public void StartToShake()
    {
        CameraShaker.Instance.ShakeOnce(m_magnitude, m_roughness, m_fadeInTime, m_fadeOutTime);
    }

}
