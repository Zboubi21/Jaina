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
        if (!PlayerManager.Instance.m_powers.m_Block.m_inIceBlock && !PlayerManager.Instance.PlayerIsDead)
        {
            base.TakeDamage(damage);
            m_lifeBar.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);

            if(CurrentHealth <= 0){
                GetComponent<PlayerManager>().On_PlayerDie();
            }
        }
    }

    public override void HealDamage(int heal)
    {
        if (CurrentHealth + heal >= maxHealth)
        {
            CurrentHealth = maxHealth;
        }
        else
        {
            CurrentHealth += heal;
        }
        m_lifeBar.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);

    }
    public override void FullHeal()
    {
        CurrentHealth = maxHealth;
        m_lifeBar.fillAmount = Mathf.InverseLerp(0, maxHealth, CurrentHealth);
    }

}
