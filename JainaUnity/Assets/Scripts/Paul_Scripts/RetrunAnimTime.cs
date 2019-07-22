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

    public void Call_Butcher_Impact()
    {
        controller.GetComponent<EnemyController>().OnImpactDamage();
    }

    public void Call_ZglorgetteAttack()
    {
        controller.GetComponent<EnemyController>().OnCastProjectil();
    }

    public void Call_ZglorgetteAttack_Impatience()
    {
        controller.GetComponent<EnemyController>().OnCastImpatienceProjectil();
    }
}
