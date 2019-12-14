using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class StalactiteSpawnManager : MonoBehaviour
{
    public Transform[] possibleSlot;
    //[Space]
    //public GameObject stalactitePrefab;
    [Space]
    public int currentPhase;
    [Space]
    public int[] nbrOfStalactitePerPhase;
    public int[] nbrOfCristilizeStalactitePerPhase;
    [Space]
    [Range(0,100)]
    public float chanceForAStalactiteToBeCristilized;
    int _currentSpawnedStalactite;
    int _currentCristilizeStalactite;
    [Space]
    public float minTimeBeforeStalactiteFall = 0f;
    public float maxTimeBeforeStalactiteFall = 1f;
    List<int> usedSlots = new List<int>();
    List<int> possibleSlotInts = new List<int>();
    List<int> lavaSlots = new List<int>();
    int nbrOfFreeSlots;
    int randomSlots;

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;

        for (int i = 0, l = possibleSlot.Length; i < l; ++i)
        {
            possibleSlotInts.Add(i);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if(usedSlots.Count != 0)
            {
                nbrOfFreeSlots = possibleSlot.Length - usedSlots.Count;
            }
            else
            {
                nbrOfFreeSlots = possibleSlot.Length;
            }

            if(nbrOfFreeSlots >= nbrOfStalactitePerPhase[currentPhase])
            {
                StartCoroutine(SpawnStalactite(nbrOfStalactitePerPhase[currentPhase]));
            }
            else
            {
                StartCoroutine(SpawnStalactite(nbrOfFreeSlots));
            }
        }
    }

    IEnumerator SpawnStalactite(int loop)
    {
        for (int i = 0, l = loop; i < l; ++i)
        {
            float randomTime = Random.Range(minTimeBeforeStalactiteFall, maxTimeBeforeStalactiteFall);
            yield return new WaitForSeconds(randomTime);
            int index = Random.Range(0, possibleSlotInts.Count);
            usedSlots.Add(possibleSlotInts[index]);


            float isCristilize = Random.Range(0, 100);

            if ((_currentCristilizeStalactite != nbrOfCristilizeStalactitePerPhase[currentPhase] && isCristilize <= chanceForAStalactiteToBeCristilized) || (nbrOfStalactitePerPhase[currentPhase] - _currentSpawnedStalactite == nbrOfCristilizeStalactitePerPhase[currentPhase]))
            {
                _currentCristilizeStalactite++;
                GenerateStalactite(index, true);
            }
            else
            {
                _currentSpawnedStalactite++;
                GenerateStalactite(index, false);
            }
            possibleSlotInts.RemoveAt(index);
        }

        _currentCristilizeStalactite = 0;

    }

    void GenerateStalactite(int index, bool hasToCristilize)
    {
        GameObject go = m_objectPooler.SpawnEnemyFromPool(EnemyType.Stalactite, possibleSlot[possibleSlotInts[index]].position, Quaternion.identity);

        StalactiteSpawnerTracker tracker = go.AddComponent<StalactiteSpawnerTracker>(); //Tant que JJ boss sur le prefab

        tracker.intSlotPosition = possibleSlotInts[index];
        tracker.spawnManager = this;
        tracker.isCristilize = hasToCristilize;

        if(lavaSlots.Count != 0)
        {
            for (int i = 0, l = lavaSlots.Count; i < l; ++i)
            {
                if(possibleSlotInts[index] == lavaSlots[i])
                {
                    tracker.isInLava = true;
                    break;
                }
                tracker.isInLava = false;
            }
        }
    }

    public void StalactiteHasBeenDestroyed(int pos, bool hasExploded)
    {
        if (hasExploded)
        {
            lavaSlots.Add(pos);
        }
        possibleSlotInts.Add(pos);
        usedSlots.Remove(pos);
    }
}
