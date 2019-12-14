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
            StartCoroutine(CreatRandomSlots(nbrOfStalactiteToSpawn, hasToEnterFusion));
        }
        else
        {
            StartCoroutine(CreatRandomSlots(nbrOfFreeSlots, hasToEnterFusion));
        }
    }

    IEnumerator CreatRandomSlots(int nbrOfSlots, bool hasToEnterFusion)
    {
        for (int i = 0, l = nbrOfSlots; i < l; ++i)
        {
            float randomTime = Random.Range(minTimeBeforeStalactiteFall, maxTimeBeforeStalactiteFall);
            int index = Random.Range(0, possibleSlotInts.Count);
            usedSlots.Add(possibleSlotInts[index]);


            float isCristilize = Random.Range(0, 100);

            yield return new WaitForSeconds(randomTime);
            if ((_currentCristilizeStalactite != nbrOfCristilizeStalactitePerPhase[_phaseForArray] && isCristilize <= chanceForAStalactiteToBeCristilized) || (nbrOfStalactitePerPhase[_phaseForArray] - _currentSpawnedStalactite == nbrOfCristilizeStalactitePerPhase[_phaseForArray]))
            {
                _currentCristilizeStalactite++;
                SpawnFromPooler(index, true, hasToEnterFusion);
            }
            else
            {
                _currentSpawnedStalactite++;
                SpawnFromPooler(index, false, hasToEnterFusion);
            }
            possibleSlotInts.RemoveAt(index);
        }

        _currentCristilizeStalactite = 0;
        _currentSpawnedStalactite = 0;
    }


    void SpawnFromPooler(int index, bool hasToCristilize, bool hasToEnterFusion)
    {
        GameObject go = m_objectPooler.SpawnEnemyFromPool(EnemyType.Stalactite, possibleSlot[possibleSlotInts[index]].position, Quaternion.identity);

        StalactiteController control = go.GetComponent<StalactiteController>(); //Tant que JJ boss sur le prefab
        control.StartFallingStalactite();

        control.IntSlotPosition = possibleSlotInts[index];
        control.SpawnManager = this;

        control.m_cristals.cristalsParent.SetActive(hasToCristilize);
        control.IsCristilize = hasToCristilize;

        if (hasToEnterFusion)
        {
            control.AddStalactiteState();
        }

        if(lavaSlots.Count != 0)
        {
            for (int i = 0, l = lavaSlots.Count; i < l; ++i)
            {
                if(possibleSlotInts[index] == lavaSlots[i])
                {
                    control.IsInLava = true;
                    break;
                }
                control.IsInLava = false;
            }
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
    }
}
