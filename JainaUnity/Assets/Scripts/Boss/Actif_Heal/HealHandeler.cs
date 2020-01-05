using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealHandeler : MonoBehaviour
{
    public bool useHealUI;
    [Space]
    public int healAmount;
    [Space]
    public int maxHealCount;
    int _currentHealCount;
    [Space]
    public float healCooldown;
    float _currenthealCooldown;
    [Space]
    public GameObject m_healSFX;
    public GameObject m_healVFX;

    ReferenceScript _getImg;
    CharacterStats stats;

    private void Start()
    {
        _getImg = GetComponent<ReferenceScript>();
        _currentHealCount = maxHealCount;

        if (!useHealUI)
        {
            gameObject.SetActive(false);
        }

        stats = PlayerManager.Instance.GetComponent<CharacterStats>();
    }


    private void Update()
    {
        HealCoolDownHandeler();
    }

    public void HealEffect()
    {
        if(!useHealUI)
        {
            return;
        }
        if (_currentHealCount - 1 >= 0 && stats.HealDamage(healAmount))
        {
            _currentHealCount--;
            Level.AddFX(m_healSFX, Vector3.zero, Quaternion.identity);
            Level.AddFX(m_healVFX, stats.gameObject.transform.position, stats.gameObject.transform.rotation);
        }
    }

    public void HealCoolDownHandeler()
    {
        if (_currentHealCount < maxHealCount)
        {
            _currenthealCooldown += Time.deltaTime;

            if (_currentHealCount > 0)
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

        _getImg.count.text = string.Format("x {0}", _currentHealCount);

    }
}
