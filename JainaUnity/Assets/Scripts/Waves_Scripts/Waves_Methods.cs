﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Waves_Methods : MonoBehaviour
{
    public GameObject[] m_spawners;
    [Space]

    public float[] m_timeBetweenEachWave;
    [Space]

    public float timeToSpawn = 0.5f;
    [Space]

    public UnityEvent OnFirstWaveStart;
    public UnityEvent OnAnyWaveStart;
    public UnityEvent OnLastWaveOver;

    Spawner_Methods[] _spawnerMethod;
    int nbrOfWave;
    int NombreDeVague;
    float timeNextWave;

    bool _playerOnTrigger;

    int nbrOfEnemy;
    int nbrEnemyDead;

    #region Get Set
    public int NbrOfEnemy
    {
        get
        {
            return nbrOfEnemy;
        }

        set
        {
            nbrOfEnemy = value;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_playerOnTrigger)
            {
                Spawner(nbrOfWave);
                _playerOnTrigger = true;
                OnFirstWaveStart.Invoke();
            }
        }
    }

    private void Start()
    {
        timeNextWave = m_timeBetweenEachWave[0];
        _spawnerMethod = new Spawner_Methods[m_spawners.Length];
        for (int a = 0, f = m_spawners.Length; a < f; a++)
        {
            if(m_spawners != null)
            {
                _spawnerMethod[a] = m_spawners[a].GetComponent<Spawner_Methods>();
            }
        }
        for (int i = 0, l = m_spawners.Length; i < l; ++i)
        {
            if (m_spawners != null)
            {
                if (m_spawners.Length-1 != i)
                {
                    if (_spawnerMethod[i]._nbrOfWaves.Length != _spawnerMethod[i+1]._nbrOfWaves.Length)
                    {
                        Debug.LogError("Le spawner " + _spawnerMethod[i] + " a un nombre de vagues différentes du spawner " + _spawnerMethod[i - 1] + ". Tous les spawners d'une arène doivent avoir le même nombre de vague");
                    }
                    else
                    {
                        NombreDeVague = _spawnerMethod[i]._nbrOfWaves.Length;
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (_playerOnTrigger && nbrOfWave != NombreDeVague)
        {
            timeNextWave -= Time.deltaTime;
            if(timeNextWave <= 0)
            {
                Spawner(nbrOfWave);
            }

            if(nbrOfEnemy == nbrEnemyDead && nbrEnemyDead != 0)
            {
                Spawner(nbrOfWave);
            }
        }
        else if (nbrOfWave == NombreDeVague && nbrOfEnemy == nbrEnemyDead && nbrOfEnemy !=0)
        {
            OnLastWaveOver.Invoke();
        }

    }

    public void CountDeath()
    {
        nbrEnemyDead++;
    }

    void Spawner(int wave)
    {
        //nbrEnemyDead = 0;
        //nbrOfEnemy = 0;
        OnAnyWaveStart.Invoke();
        if (wave > m_timeBetweenEachWave.Length-1)
        {
            timeNextWave = m_timeBetweenEachWave[wave-1];
        }
        else
        {
            timeNextWave = m_timeBetweenEachWave[wave];
        }
        for (int i = 0, l = m_spawners.Length; i < l; ++i)
        {
            if (m_spawners[i] != null)
            {
                _spawnerMethod[i].StartCoroutine(_spawnerMethod[i].WaveSpawner(i, wave, this));
            }
        }
        nbrOfWave++;
    }
}
