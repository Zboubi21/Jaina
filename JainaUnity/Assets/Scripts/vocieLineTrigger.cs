using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vocieLineTrigger : MonoBehaviour
{
    public GameObject voiceLine;
    public float timeBeforeRepeatingTheLine;


    bool canRepeating = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canRepeating)
        {
            Level.AddFX(voiceLine, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && canRepeating)
        {
            canRepeating = false;
            StartCoroutine(waitBeforeRepeating());
        }
    }


    IEnumerator waitBeforeRepeating()
    {
        yield return new WaitForSeconds(timeBeforeRepeatingTheLine);
        canRepeating = true;
    }
}
