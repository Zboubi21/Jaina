using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using EnemyStateEnum;

public class GolemStats : EnemyStats
{
    public override void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    public override void Start()
    {
    }
    public override void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(4999);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1000);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(100);
        }
#endif
    }
    public override void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        Debug.Log("aie");
    }
}
