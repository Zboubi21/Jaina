using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSpawnerGizmos : MonoBehaviour
{
    public float impactRadius;
    public float lavaRadius;
    public bool overrideColor;
    public Color c_impactRadius;
    public Color c_lavaRadius;

    public bool isClickable;

    private void OnDrawGizmos()
    {
        if (!overrideColor)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, impactRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lavaRadius);
        }
        else
        {
            Gizmos.color = c_impactRadius;
            Gizmos.DrawWireSphere(transform.position, impactRadius);

            Gizmos.color = c_lavaRadius;
            Gizmos.DrawWireSphere(transform.position, lavaRadius);
        }
        if(gameObject.GetComponent<MeshRenderer>() != null)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = isClickable;
        }

    }

    private void Start()
    {
        isClickable = false;
    }
}
