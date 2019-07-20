using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterStats : MonoBehaviour {

    public float maxHealth = 100;
    [Range(0,100)]
    public float m_currentHealth = 100;

    public Stats damage;
    public Stats armor;

    [HideInInspector]
    public Camera mainCamera;

    #region Get Set
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }

        set
        {
            currentHealth = value;
        }
    }

    public int ArcanMarkCount
    {
        get
        {
            return arcanMarkCount;
        }

        set
        {
            arcanMarkCount = value;
        }
    }

    public int FireMarkCount
    {
        get
        {
            return fireMarkCount;
        }

        set
        {
            fireMarkCount = value;
        }
    }

    public int GivreMarkCount
    {
        get
        {
            return givreMarkCount;
        }

        set
        {
            givreMarkCount = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDead = value;
        }
    }

    public int MaxArcanMarkCount
    {
        get
        {
            return maxArcanMarkCount;
        }

        set
        {
            maxArcanMarkCount = value;
        }
    }
    #endregion
    private float currentHealth;// { get; private set; }
    int arcanMarkCount;
    int fireMarkCount;
    int givreMarkCount;

    float bonusDamage = 1f;

    float multiplicateur;
    float arcanBlastMultiplicateur;

    float previousDamage = 1;

    int maxArcanMarkCount;

    bool isDead = false;

    public virtual void OnEnable()
    {
        if(mainCamera == null){
    mainCamera = Camera.main;//.GetComponent<Camera>();
        }
    }

    public virtual void Start()
    {

        // mainCamera = Camera.main;//.GetComponent<Camera>();
        CurrentHealth = maxHealth * (m_currentHealth /100f);

        multiplicateur = PlayerManager.Instance.m_percentMultiplicateur / 100f;
        arcanBlastMultiplicateur = PlayerManager.Instance.m_powers.m_arcaneExplosion.m_blastMultiplicateur / 100f;
        maxArcanMarkCount = PlayerManager.Instance.m_maxArcanMarkCount;
        //Debug.Log(multiplicateur);
    }

    public virtual void Update()
    {
        
    }

    #region Mark Methods
    public virtual void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        if(ArcanMarkCount + nbrMarks < maxArcanMarkCount)
        {
            ArcanMarkCount += nbrMarks;
        }
        else if (ArcanMarkCount + nbrMarks >= maxArcanMarkCount)
        {
            ArcanMarkCount = maxArcanMarkCount;
        }
        //TakeDamage((int)(damage*(1 + multiplicateur * ArcanMarkCount)));
        float dmg = (previousDamage * multiplicateur * ArcanMarkCount) + damage;
        previousDamage = dmg;
        TakeDamage((int)dmg);
    }
    public virtual void AutoAttackArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        if (ArcanMarkCount < maxArcanMarkCount)
        {
            ArcanMarkCount++;
        }
        float dmg = (previousDamage * multiplicateur * ArcanMarkCount) + damage;
        previousDamage = dmg;
        TakeDamage((int)dmg);
    }
    public virtual void AutoAttackFireMark(float timerDebuf)
    {
        if (FireMarkCount == 0)
        {
            FireMarkCount++;
        }
    }
    public virtual void FireMark(float timerDebuf)
    {
        if (FireMarkCount <= 2)
        {
            FireMarkCount++;
        }
    }
    public virtual void IceMark(float timerDebuf)
    {
        if (GivreMarkCount < 5)
        {
            GivreMarkCount++;
        }
    }
    public virtual void ArcaneExplosion(int damage)
    {
        bonusDamage = (GivreMarkCount + FireMarkCount + ArcanMarkCount) * arcanBlastMultiplicateur;
        //Debug.Log("YAYAYAAYYAAY" + bonusDamage);
        TakeDamage((int)(damage + (damage * bonusDamage)));
        ArcanMarkCount = FireMarkCount = GivreMarkCount = 0;
        bonusDamage = 1;
    }

    #endregion

    public virtual void OnEnemyInCombatCount()
    {

    }

    public virtual void OnEnemyKillCount()
    {

    }


    // public virtual void ArcaneProjectil(int damage,float timerDebuf, int nbrMarks)
    // {
    //     if (ArcanMarkCount < MaxArcanMarkCount)
    //     {
    //         ArcanMarkCount += nbrMarks;
    //     }
    //     TakeDamage((int)(damage + (damage * ((ArcanMarkCount) * multiplicateur))));
    // }


    public virtual void IceNova()
    {

    }

    public virtual void FireTrail()
    {

    }

    public virtual void TakeDamage (int damage)
    {
        
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);


        // Debug.Log(transform.name + " takes " + damage + " damages.");
        //Debug.Log(transform.name + "n'a plus que :" + CurrentHealth + " point de vie.");

        if (CurrentHealth > 0)
        {
            CurrentHealth -= damage;
        }

        CheckIfHasToDie(CurrentHealth);
    }
    public virtual void HealDamage(int heal)
    {

    }
    public virtual void FullHeal()
    {

    }

    void CheckIfHasToDie(float health)
    {
        if (health <= 0)
        {
            if (!IsDead)
            {
                Die();
                IsDead = true;
            }
        }
    }

    public virtual void Die()
    {
        // Debug.Log(transform.name + " died.");
    }

    

}
