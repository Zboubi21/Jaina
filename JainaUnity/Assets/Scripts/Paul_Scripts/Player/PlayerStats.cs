using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats {

    [SerializeField] Image m_lifeBar;

    public override void Die()
    {
        base.Die();

        //Debug.Log("Jaina is dead !!");
    }
    public override void Start(){
        base.Start();
        m_lifeBar.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
    }

    public override void TakeDamage(int damage)
    {
        if (!PlayerManager.Instance.m_powers.m_Block.m_inIceBlock)
        {
            base.TakeDamage(damage);
            m_lifeBar.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
        }
    }

}
