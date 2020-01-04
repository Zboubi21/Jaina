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
    [Range(1,3)]
    public int currentPhase;
    int _phaseForArray;
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

    float timeToWaitUntilLastStalactiteHasFallen;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;

        _phaseForArray = currentPhase - 1;

        for (int i = 0, l = possibleSlot.Length; i < l; ++i)
        {
            possibleSlotInts.Add(i);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {

            OnGenerateStalactite(nbrOfStalactitePerPhase[_phaseForArray], false);   //Generation for the stalactite patern

        }

        if (Input.GetKeyDown(KeyCode.X))
        {

            OnGenerateStalactite(nbrOfStalactitePerPhase[_phaseForArray], true);   //Generation for the smash patern in P3

        }
    }

    public void OnGenerateStalactite(int nbrOfStalactiteToSpawn, bool hasToEnterFusion)
    {
        if (usedSlots.Count != 0)
        {
            nbrOfFreeSlots = possibleSlot.Length - usedSlots.Count;
        }
        else
        {
            nbrOfFreeSlots = possibleSlot.Length;
        }

        if (nbrOfFreeSlots >= nbrOfStalactiteToSpawn)
        {
            LockSlotsForStalactite(nbrOfStalactiteToSpawn, hasToEnterFusion);
        }
        else
        {
            LockSlotsForStalactite(nbrOfFreeSlots, hasToEnterFusion);
        }
    }
    public void StalactiteHasBeenDestroyed(int pos, bool hasToCreateLava)
    {
        if (hasToCreateLava)
        {
            lavaSlots.Add(pos);
        }
        possibleSlotInts.Add(pos);
        usedSlots.Remove(pos);
        _countStalactite--;
    }

    #region LockSlotsMethods
    int i;
    void LockSlotsForStalactite(int nbrOfSlots, bool hasToEnterFusion)
    {
        while(i < nbrOfSlots)
        {
            i++;
            int indexToSpawn = Random.Range(0, possibleSlotInts.Count);
            int intSlotToSpawn = possibleSlotInts[indexToSpawn];

            usedSlots.Add(possibleSlotInts[indexToSpawn]);
            possibleSlotInts.RemoveAt(indexToSpawn);

            if (HasToCristilize())
            {
                _currentCristilizeStalactite++;
                StartCoroutine(SpawnStalactiteOnSlots(intSlotToSpawn, true, hasToEnterFusion));
            }
            else
            {
                _currentSpawnedStalactite++;
                StartCoroutine(SpawnStalactiteOnSlots(intSlotToSpawn, false, hasToEnterFusion));
            }
        }

        i = 0;
        _currentCristilizeStalactite = 0;
        _currentSpawnedStalactite = 0;


    }
    #endregion

    #region Spawn Stalactite Corout
    int _countStalactite;
    IEnumerator SpawnStalactiteOnSlots(int indexToSpawn, bool isCristilized, bool hasToEnterFusion)
    {
        float randomTime = Random.Range(minTimeBeforeStalactiteFall, maxTimeBeforeStalactiteFall);
        yield return new WaitForSeconds(randomTime);
        SpawnFromPooler(indexToSpawn, isCristilized, hasToEnterFusion);
        _countStalactite++;
        if (_countStalactite == usedSlots.Count)
        {
            float time = timeToWaitUntilLastStalactiteHasFallen;
            StartCoroutine(WaitUntilLastStalactilHasFallen(time));
        }
    }
    #endregion

    bool HasToCristilize()
    {
        float isCristilize = Random.Range(0, 100);
        if((_currentCristilizeStalactite != nbrOfCristilizeStalactitePerPhase[_phaseForArray] && isCristilize <= chanceForAStalactiteToBeCristilized) || (nbrOfStalactitePerPhase[_phaseForArray] - _currentSpawnedStalactite == nbrOfCristilizeStalactitePerPhase[_phaseForArray]))
        {
            return true;
        }
        return false;
    }

    void SpawnFromPooler(int index, bool hasToCristilize, bool hasToEnterFusion)
    {
        GameObject go = m_objectPooler.SpawnEnemyFromPool(EnemyType.Stalactite, possibleSlot[index].position, Quaternion.identity);
        StalactiteController control = go.GetComponent<StalactiteController>();
        StalactiteStats stats = go.GetComponent<StalactiteStats>();
        stats.CurrentHealth = stats.maxHealth;
        stats.IsDead = false;
        control.StartFallingStalactite();

        control.IntSlotPosition = index;
        control.SpawnManager = this;

        control.m_cristals.cristalsParent.SetActive(hasToCristilize);
        control.IsCristilize = hasToCristilize;

        timeToWaitUntilLastStalactiteHasFallen = control.m_moveAnimation.m_timeToReachPosition + control.m_sign.m_timetoFallStalactite;

        if (hasToEnterFusion)
        {
            control.AddStalactiteState();
        }

        if(lavaSlots.Count != 0)
        {
            for (int i = 0, l = lavaSlots.Count; i < l; ++i)
            {
                if(index == lavaSlots[i])
                {
                    control.IsInLava = true;
                    break;
                }
                control.IsInLava = false;
            }
        }
    }

    IEnumerator WaitUntilLastStalactilHasFallen(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("J'AI FINI DE FAIRE TOMBER LES STALACTITES !!!!");
        // J'AI FINI DE FAIRE TOMBER LES STALACTITES !!!!

    }

}
