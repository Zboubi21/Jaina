using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class ClickOnGround : MonoBehaviour {
    
    [SerializeField] float m_timeToBeVisible = 1;

    ObjectPooler m_objectPooler;
    IEnumerator corout;

    void OnEnable(){
        if(corout != null){
            StopCoroutine(corout);
        }
        corout = LifeTime();
        StartCoroutine(corout);
    }
    
    void Start(){
        m_objectPooler = ObjectPooler.Instance;
    }

    void OnDisable(){
        if(corout != null){
            StopCoroutine(corout);
        }
    }

    IEnumerator LifeTime(){
        yield return new WaitForSeconds(m_timeToBeVisible);
        DestroyFx();
    }

    public void DestroyFx(){
        if(corout != null){
            StopCoroutine(corout);
        }
        m_objectPooler.ReturnObjectToPool(ObjectType.ClickOnGround, gameObject);
    }

}
