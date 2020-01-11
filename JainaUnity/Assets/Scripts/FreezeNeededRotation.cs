using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeNeededRotation : MonoBehaviour
{
    
    [SerializeField] Vector3 m_neededRotation;

    void LateUpdate()
    {
        transform.eulerAngles = m_neededRotation;
    }
    
}
