using UnityEngine;

public class ObjectFaceMainCamera : MonoBehaviour {
    
    CameraManager m_mainCamera;

    void Start(){
        m_mainCamera = CameraManager.Instance;
    }

    void Update(){
        transform.LookAt(m_mainCamera.transform);
    }

}
