using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecameInvisible : MonoBehaviour {

    // [SerializeField] float m_timeToDeleteProjectileWhenHeComeInvisible = 0;

    bool m_timeToDestroy = false;
    public bool TimeToDestroy{
        get{
            return m_timeToDestroy;
        }
    }

    void OnEnable(){
        // StopAllCoroutines();
        m_timeToDestroy = false;
    }

    // void OnBecameVisible() {
    //     StopAllCoroutines();
    //     // Debug.Log(gameObject.name + " deviens visible");
    // }

    void OnBecameInvisible() {
        // StartCoroutine(WaitDeleteTime());
        // Debug.Log(gameObject.name + " deviens invisible");
        m_timeToDestroy = true;
    }

    // IEnumerator WaitDeleteTime(){
    //     yield return new WaitForSeconds(m_timeToDeleteProjectileWhenHeComeInvisible);
    //     m_timeToDestroy = true;
    // }

}
