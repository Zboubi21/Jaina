using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSpawnerGizmos : MonoBehaviour
{
    public float impactRadius;
    public float lavaRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lavaRadius);

    }
}
