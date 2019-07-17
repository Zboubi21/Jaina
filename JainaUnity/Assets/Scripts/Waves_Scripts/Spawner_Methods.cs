using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class Spawner_Methods : MonoBehaviour
{
    [System.Serializable]
    public class WavesList
    {
        [System.Serializable]
        public class TypeOfEnemy
        {
            public bool _hasBackpack;
            public EnemyType m_enemy;
        }
        public TypeOfEnemy[] m_enemyToSummon;
    }
    public WavesList[] _nbrOfWaves;

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    public IEnumerator WaveSpawner(int i, int wave, Waves_Methods methods)
    {
        for (int a = 0, f = _nbrOfWaves[wave].m_enemyToSummon.Length; a < f; ++a)
        {
            yield return new WaitForSeconds(methods.timeToSpawn);
            //GameObject go = Instantiate(_nbrOfWaves[wave].m_enemyToSummon[a], transform);
            GameObject go = m_objectPooler.SpawnEnemyFromPool(_nbrOfWaves[wave].m_enemyToSummon[a].m_enemy, transform.position, transform.rotation);
            go.AddComponent<Spawned_Tracker>();
            go.GetComponent<Spawned_Tracker>().wave = methods;
            if (_nbrOfWaves[wave].m_enemyToSummon[a]._hasBackpack)
            {
                go.GetComponent<EnemyStats>()._hasBackPack = true;
            }
            else if (!_nbrOfWaves[wave].m_enemyToSummon[a]._hasBackpack)
            {
                go.GetComponent<EnemyStats>()._hasBackPack = false;
            }
            methods.NbrOfEnemy++;
        }
        StopCoroutine(WaveSpawner(i, wave, methods));
    }
}
