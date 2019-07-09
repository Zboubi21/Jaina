﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Methods : MonoBehaviour
{
    [System.Serializable]
    public class WavesList
    {
        public GameObject[] m_enemyToSummon;
    }
    public WavesList[] _nbrOfWaves;


    public IEnumerator WaveSpawner(int i, int wave, Waves_Methods methods)
    {
        for (int a = 0, f = _nbrOfWaves[wave].m_enemyToSummon.Length; a < f; ++a)
        {
            yield return new WaitForSeconds(methods.timeToSpawn);
            GameObject go = Instantiate(_nbrOfWaves[wave].m_enemyToSummon[a], transform);
            go.AddComponent<Spawned_Tracker>();
            go.GetComponent<Spawned_Tracker>().wave = methods;
            methods.NbrOfEnemy++;
        }
        StopCoroutine(WaveSpawner(i, wave, methods));
    }
}