using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Beam_Spawner_Controller : MonoBehaviour
{
    public GameObject actifVFX;
    public Transform startPivot;
    public Transform pivotPoint;
    public Image sign;
    public Image butchSign;

    
    bool hasToLookAt = true;
    int _nbrOfShoot;

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

    public int NbrOfShoot
    {
        get
        {
            return _nbrOfShoot;
        }

        set
        {
            _nbrOfShoot = value;
        }
    }
    #endregion


}
