using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraPosition : MonoBehaviour {
    
    CameraManager m_cameraManager;
    Transform m_targetTransform;

    void Start(){
        m_cameraManager = CameraManager.Instance;
        m_targetTransform = m_cameraManager.transform;
    }

    void FixedUpdate(){
        transform.position = new Vector3(m_targetTransform.position.x, transform.position.y, m_targetTransform.position.z);
    }

}
