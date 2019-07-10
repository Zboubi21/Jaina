using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class Spawner_Methods : MonoBehaviour
{
    [System.Serializable]
    public class WavesList
    {
        public GameObject[] m_enemyToSummon;
        public EnemyType[] m_enemyToSummon1;
    }
    public WavesList[] _nbrOfWaves;

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    public IEnumerator WaveSpawner(int i, int wave, Waves_Methods methods)
    {
        for (int a = 0, f = _nbrOfWaves[wave].m_enemyToSummon1.Length; a < f; ++a)
        {
            yield return new WaitForSeconds(methods.timeToSpawn);
            //GameObject go = Instantiate(_nbrOfWaves[wave].m_enemyToSummon[a], transform);
            GameObject go = m_objectPooler.SpawnEnemyFromPool(_nbrOfWaves[wave].m_enemyToSummon1[i], transform.position, transform.rotation);
            go.AddComponent<Spawned_Tracker>();
            go.GetComponent<Spawned_Tracker>().wave = methods;
            methods.NbrOfEnemy++;
        }
        StopCoroutine(WaveSpawner(i, wave, methods));
    }
}
