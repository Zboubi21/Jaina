using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmedialLightReference : MonoBehaviour
{
    public Light[] lights;
    public Material[] mats;
    public Transform VFX_Spawn;

    public void ResetLightPos()
    {
        for (int i = 0, l = lights.Length; i < l; ++i)
        {
            ArmedialLightPathReference lightPath = lights[i].GetComponent<ArmedialLightPathReference>();
            lights[i].transform.position = lightPath.dieBossPoint.position;
        }
    }

}
