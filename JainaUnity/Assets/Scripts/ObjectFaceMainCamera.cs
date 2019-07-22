using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFaceMainCamera : MonoBehaviour {
    
    CameraManager m_mainCamera;

    void OnEnable(){
        m_mainCamera = CameraManager.Instance;
    }

    void Start(){
        m_mainCamera = CameraManager.Instance;
    }

    void Update(){
        transform.LookAt(m_mainCamera.transform);
    }

}
