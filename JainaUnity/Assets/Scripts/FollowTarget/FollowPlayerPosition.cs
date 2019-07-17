using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour {
    
    PlayerManager m_playerManager;
    Transform m_targetTransform;

    void Start(){
        m_playerManager = PlayerManager.Instance;
        m_targetTransform = m_playerManager.transform;
    }

    void FixedUpdate(){
        transform.position = m_targetTransform.position;
    }

}
