using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBecameInvisible : MonoBehaviour {
    
    [SerializeField] bool m_isActive = true;
    [SerializeField] float m_additionalTimer = 0;
    [SerializeField] UnityEvent m_onBecameInvisible;


    void OnEnable(){
        StopAllCoroutines();
    }

    // void OnBecameVisible() {
    //     StopAllCoroutines();
    //     // Debug.Log(gameObject.name + " deviens visible");
    // }

    void OnBecameInvisible() {
        Debug.Log(gameObject.name + " deviens invisible");
        if(m_isActive){
            // StartCoroutine(WaitDeleteTime());
            m_onBecameInvisible.Invoke();
        }
    }

    // IEnumerator WaitDeleteTime(){
    //     // yield return new WaitForSeconds(m_additionalTimer);
    //     m_onBecameInvisible.Invoke();
    // }

    public void ActiveCheckBecameInvisible(bool b){
        m_isActive = b;
    }

}
