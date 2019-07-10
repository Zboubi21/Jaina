using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Waves_Methods : MonoBehaviour
{
    public GameObject[] m_spawners;
    public float[] m_timeBetweenEachWave;
    public float timeToSpawn = 0.5f;
    public int NombreDeVague;

    public UnityEvent OnFirstWaveStart;
    public UnityEvent OnLastWaveOver;

    Spawner_Methods[] _spawnerMethod;
    int nbrOfWave;
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
        for (int i = 0, l = m_spawners.Length; i < l; ++i)
        {
            if (m_spawners != null)
            {
                _spawnerMethod[i] = m_spawners[i].GetComponent<Spawner_Methods>();
                if(_spawnerMethod[i]._nbrOfWaves.Length > NombreDeVague)
                {
                    Debug.LogError("Le nombre de vague sur le spawner : " + _spawnerMethod[i].name + " a plus de vague que le nombre de vague référencé sur ce script");
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

            if(nbrOfEnemy == nbrEnemyDead && nbrEnemyDead !=0)
            {
                Spawner(nbrOfWave);
            }
        }
        else if (nbrOfWave == NombreDeVague && nbrOfEnemy == nbrEnemyDead)
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
        nbrEnemyDead = 0;
        nbrOfEnemy = 0;
        if(wave > m_timeBetweenEachWave.Length-1)
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
