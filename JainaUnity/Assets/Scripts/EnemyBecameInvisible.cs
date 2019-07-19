using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBecameInvisible : MonoBehaviour {
    
    [SerializeField] float m_additionalTimer = 0;
    [SerializeField] UnityEvent m_onBecameInvisible;

    bool m_isActive = true;
    public bool IsActive{
        get{
            return m_isActive;
        }
        set{
            m_isActive = value;
        }
    }

    void OnEnable(){
        StopAllCoroutines();
    }

    // void OnBecameVisible() {
    //     StopAllCoroutines();
    //     // Debug.Log(gameObject.name + " deviens visible");
    // }

    void OnBecameInvisible() {
        if(m_isActive){
            StartCoroutine(WaitDeleteTime());
            // Debug.Log(gameObject.name + " deviens invisible");
        }
    }

    IEnumerator WaitDeleteTime(){
        yield return new WaitForSeconds(m_additionalTimer);
        m_onBecameInvisible.Invoke();
    }

    public void ActiveCheckBecameInvisible(bool b){
        m_isActive = b;
    }

}
