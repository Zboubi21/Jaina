using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSmoke : MonoBehaviour
{

    public void On_ChangeSmoke()
    {
        GetComponent<ParticleSystem>().loop = false;
    }

}
