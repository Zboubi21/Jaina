using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrunAnimTime : MonoBehaviour {

    EnemyController controller;

    private void Start()
    {
        controller = GetComponentInParent<EnemyController>();
    }

    public void CallFunction()
    {
        controller.GetComponent<EnemyController>().Timed();
        
    }
}
