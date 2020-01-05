﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Beam_Spawner_Controller : MonoBehaviour
{
    public GameObject actifVFX;
    public Transform pivotPoint;
    public Image sign;
    
    bool hasToLookAt = true;

    #region Get Set
    public bool HasToLookAt
    {
        get
        {
            return hasToLookAt;
        }

        set
        {
            hasToLookAt = value;
        }
    }
    #endregion


}