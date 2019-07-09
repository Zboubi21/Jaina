using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawned_Tracker : MonoBehaviour
{

    public Waves_Methods wave;

    EnemyStats stats;

    private void OnEnable()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        stats = GetComponent<EnemyStats>();
    }
    /*bool go;
    private void Update()
    {
        if(stats.CurrentHealth <= 0 && !go)
        {
            go = true;
        }
    }*/

    public void CallDead()
    {
        wave.CountDeath();
    }
}
