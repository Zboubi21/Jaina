using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerStats : CharacterStats {

    [SerializeField] Image m_lifeBar;

    int enemyInCombat;
    int enemyKillCount;

    bool isInCombat;
    [Space]
    [Header("Door To Open/Close When Out/In Combat")]
    public Animator[] doors;
    [Space]
    public string openDoor;
    public string closeDoor;
    [Space]
    [Header("Event Triggering")]
    public UnityEvent InCombatEvent;
    public UnityEvent OutOfCombatEvent;

    #region get set
    public bool IsInCombat
    {
        get
        {
            return isInCombat;
        }

        set
        {
            isInCombat = value;
        }
    }
    #endregion

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

            if(CurrentHealth <= 0 && PlayerManager.Instance.m_playerDebug.m_playerCanDie){
                GetComponent<PlayerManager>().On_PlayerDie();
                enemyKillCount = 0;
                enemyInCombat = 0;
                isInCombat = false;

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

    public override void OnEnemyInCombatCount()
    {
        enemyInCombat++;
        if((enemyInCombat > 0 || isInArena) && !isInCombat)
        {
            CheckCombatOn();
        }
    }

    public override void OnEnemyKillCount()
    {
        enemyKillCount++;
        if (enemyKillCount == enemyInCombat && enemyKillCount !=0 && !isInArena)
        {
            CheckCombatOff();
        }
    }

    void CheckCombatOn()
    {
        InCombatEvent.Invoke();
        for (int i = 0, l = doors.Length; i < l; ++i)
        {
            Debug.Log("closeDoor");
            doors[i].SetTrigger(closeDoor);
        }
        isInCombat = true;
    }

    void CheckCombatOff()
    {
        enemyKillCount = 0;
        enemyInCombat = 0;
        OutOfCombatEvent.Invoke();
        for (int i = 0, l = doors.Length; i < l; ++i)
        {
            Debug.Log("openDoor");
            doors[i].SetTrigger(openDoor);
        }
        isInCombat = false;
    }

    bool isInArena;
    public void OnCheckArenaStillGoing(bool b)
    {
        isInArena = b;
        if (!b)
        {
            CheckCombatOff();
        }
    }
}
