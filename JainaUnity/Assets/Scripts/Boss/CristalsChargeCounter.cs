using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CristalsChargeCounter : MonoBehaviour
{

    [Range(0, 1)]
    public float percentDamageMultiplicator = 0.1f;
    private int cristCount;

    ReferenceScript _getImg;

    #region Get Set
    public int CristCount
    {
        get
        {
            return cristCount;
        }

        set
        {
            cristCount = value;
        }
    }
    #endregion

    private void Start()
    {
        _getImg = GetComponent<ReferenceScript>();
    }

    public void AddCristCount()
    {
        if(cristCount == 0)
        {
            _getImg.damageCount.gameObject.SetActive(true);
        }
        cristCount++;
        _getImg.count.text = string.Format("x {0}", cristCount);
        if(cristCount < 100)
        {
            _getImg.damageCount.text = string.Format("+ {0}% damage", cristCount* (percentDamageMultiplicator * 100f));
        }
        else
        {
            _getImg.damageCount.text = string.Format("Overpower");
        }
    }
}
