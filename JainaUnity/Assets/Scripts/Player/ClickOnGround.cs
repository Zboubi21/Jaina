using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnGround : MonoBehaviour {
    
    [SerializeField] float m_timeToBeVisible = 1;

    bool m_startBeDestroy = false;
    float m_timer;

    void Start(){
        m_timer = m_timeToBeVisible;
    }

    void Update(){
        if(m_startBeDestroy){
            m_timer -= Time.deltaTime;
            if(m_timer <= 0){
                DestroyFx();
            }
        }
    }

    public void StartBeDestroyed(){
        m_startBeDestroy = true;
    } 

    public void DestroyFx(){
        Destroy(gameObject);
    }

}
