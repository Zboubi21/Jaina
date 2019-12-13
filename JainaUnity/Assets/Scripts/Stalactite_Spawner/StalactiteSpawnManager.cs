using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnManager : MonoBehaviour
{
    public Transform[] possibleSlot;
    [Space]
    public GameObject stalactitePrefab;
    [Space]
    public int currentPhase;
    public int[] nbrOfStalactitePerPhase;
    [Space]
    public float minTimeBeforeStalactiteFall = 0f;
    public float maxTimeBeforeStalactiteFall = 1f;
    List<int> usedSlots = new List<int>();
    List<int> possibleSlotInts = new List<int>();
    int nbrOfFreeSlots;
    int randomSlots;

    private void Start()
    {
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
                StartCoroutine(GetRandomNumber(nbrOfStalactitePerPhase[currentPhase]));
            }
            else
            {
                StartCoroutine(GetRandomNumber(nbrOfFreeSlots));
            }
        }
    }

    IEnumerator GetRandomNumber(int loop)
    {
        for (int i = 0, l = loop; i < l; ++i)
        {
            float randomTime = Random.Range(minTimeBeforeStalactiteFall, maxTimeBeforeStalactiteFall);
            yield return new WaitForSeconds(randomTime);
            int index = Random.Range(0, possibleSlotInts.Count);
            usedSlots.Add(possibleSlotInts[index]);
            GameObject go = Instantiate(stalactitePrefab, possibleSlot[possibleSlotInts[index]].position, Quaternion.identity);
            possibleSlotInts.RemoveAt(index);
        }
    }
}
