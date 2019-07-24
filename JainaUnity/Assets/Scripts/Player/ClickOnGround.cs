using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnGround : MonoBehaviour {
    
    [SerializeField] float m_timeToBeVisible = 1;

    IEnumerator corout;

    void Start(){
        corout = LifeTime();
        StartCoroutine(corout);
    }

    // public void StartBeDestroyed(){
    //     corout = LifeTime();
    //     StartCoroutine(corout);
    // } 

    IEnumerator LifeTime(){
        yield return new WaitForSeconds(m_timeToBeVisible);
        DestroyFx();
    }

    public void DestroyFx(){
        if(corout != null){
            StopCoroutine(corout);
        }
        Destroy(gameObject);
        Debug.Log("DestroyFx");
    }

}
