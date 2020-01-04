using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteSpawnerTracker : MonoBehaviour
{
    public int intSlotPosition;
    public StalactiteSpawnManager spawnManager;
    public bool isCristilize;
    public bool isInLava;

    private void OnDisable()
    {
        Debug.Log("AIEU !!");
        spawnManager.StalactiteHasBeenDestroyed(intSlotPosition, true, GetComponent<StalactiteController>().HasSpawnInRedSlots);
    }
}
