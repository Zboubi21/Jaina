using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour {

    ObjectPooler m_objectPooler;

    void Start(){
        m_objectPooler = ObjectPooler.Instance;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            m_objectPooler.SpawnFromPool("Zglorg", Vector3.zero, Quaternion.identity);
        }
    }
     
}
