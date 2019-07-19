using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBecameInvisible : MonoBehaviour {
    
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
        StartCoroutine(WaitDeleteTime());
        // Debug.Log(gameObject.name + " deviens invisible");
    }

    IEnumerator WaitDeleteTime(){
        yield return new WaitForSeconds(m_additionalTimer);
        m_onBecameInvisible.Invoke();
    }

}
