using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHandeler : MonoBehaviour
{
    ReferenceScript _getImg;

    public int maxHealCount;
    int _currentHealCount;

    public float healCooldown;
    float _currenthealCooldown;


    private void Start()
    {
        _getImg = GetComponent<ReferenceScript>();
        _currentHealCount = maxHealCount;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(_currentHealCount -1 >= 0)
            {
                _currentHealCount--;
            }
        }

        if (_currentHealCount < maxHealCount)
        {
            _currenthealCooldown += Time.deltaTime;

            if(_currentHealCount > 0)
            {
                _getImg.marksArray[1].fillAmount = Mathf.InverseLerp(0, healCooldown, _currenthealCooldown);
            }
            else
            {
                _getImg.marksArray[1].fillAmount = 0;
                _getImg.marksArray[0].fillAmount = Mathf.InverseLerp(0, healCooldown, _currenthealCooldown);
            }

            if (_currenthealCooldown >= healCooldown)
            {
                _currenthealCooldown = 0;
                _currentHealCount++;
            }
        }
        else
        {
            _currentHealCount = maxHealCount;
        }

        _getImg.count.text = string.Format("{0}", _currentHealCount);

    }


    void HealCoolDown()
    {

    }
}
