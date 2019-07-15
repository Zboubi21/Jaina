﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarLord_MiseEnScene : MonoBehaviour
{
    public UnityEvent OnStartFight;
    public UnityEvent OnEndFight;

    EnemyController controller;
    EnemyStats stats;

    bool fightHasStart;
    bool fightHasEnded;

    void Start()
    {
        controller = GetComponent<EnemyController>();
        stats = GetComponent<EnemyStats>();
    }

    void Update()
    {
        if (controller.PlayerInLookRange() && !fightHasStart)
        {
            fightHasStart = true;
            OnStartFight.Invoke();
        }

        if (stats.CurrentHealth <= (stats.maxHealth/2) && !fightHasEnded)
        {
            fightHasEnded = true;
            OnEndFight.Invoke();
        }
    }
}
