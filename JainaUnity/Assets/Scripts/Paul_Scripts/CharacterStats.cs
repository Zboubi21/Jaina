using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterStats : MonoBehaviour {

    public int maxHealth = 100;

    public Stats damage;
    public Stats armor;

    [HideInInspector]
    public Camera mainCamera;

    #region Get Set
    public int CurrentHealth
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
    #endregion
    private int currentHealth;// { get; private set; }
    int arcanMarkCount;
    int fireMarkCount;
    int givreMarkCount;

    float bonusDamage = 1f;

    float multiplicateur;
    int MaxArcanMarkCount;

    bool isDead = false;


    public virtual void Start()
    {

        mainCamera = Camera.main;//.GetComponent<Camera>();
        CurrentHealth = maxHealth;
        multiplicateur = PlayerManager.Instance.m_percentMultiplicateur / 100f;
        MaxArcanMarkCount = PlayerManager.Instance.m_maxArcanMarkCount;
        //Debug.Log(multiplicateur);
    }

    public virtual void Update()
    {
        
    }

    #region Mark Methods

    public virtual void ArcanMark(int damage, float timerDebuf, int nbrMarks)
    {
        if (ArcanMarkCount + nbrMarks >= MaxArcanMarkCount)
        {
            ArcanMarkCount = MaxArcanMarkCount;
        }
        else if(ArcanMarkCount < MaxArcanMarkCount)
        {
            ArcanMarkCount += nbrMarks;
        }

        TakeDamage((int)(damage + (damage * ((ArcanMarkCount) * multiplicateur))));
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
        bonusDamage = (GivreMarkCount + FireMarkCount + ArcanMarkCount) * multiplicateur;
       // Debug.Log("YAYAYAAYYAAY" + bonusDamage);
        TakeDamage((int)(damage + (damage * bonusDamage)));
        ArcanMarkCount = FireMarkCount = GivreMarkCount = 0;
        bonusDamage = 1;
    }

    #endregion

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
