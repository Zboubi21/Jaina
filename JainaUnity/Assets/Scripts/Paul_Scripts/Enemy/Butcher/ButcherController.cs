using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum_Butcher;
public class ButcherController : EnemyController
{
    [Header("Warboar Impatience Variables")]
    public int numberOfJump;
    int nbrJump;
    public float CoolDownGettingImpatient = 2f;
    [Space]
    public GameObject signImpatience;
    [Space]
    public float rangeMinForJump;
    public Color minRangeColor;
    [Space]
    public float rangeMaxForJump;
    public Color maxRangeColor;
    float m_cdImpatient;
    float m_tempsJumpAnim = 3f;

    #region Get Set
    public float CdImpatient
    {
        get
        {
            return m_cdImpatient;
        }

        set
        {
            m_cdImpatient = value;
        }
    }

    public int NbrJump
    {
        get
        {
            return nbrJump;
        }

        set
        {
            nbrJump = value;
        }
    }

    public float TempsJumpAnim
    {
        get
        {
            return m_tempsJumpAnim;
        }

        set
        {
            m_tempsJumpAnim = value;
        }
    }
    #endregion
    public override void LogicAtStart()
    {
        base.LogicAtStart();

        m_cdImpatient = CoolDownGettingImpatient;
    }

    private void Awake()
    {
        m_sM.AddStates(new List<IState> {
            new Butcher_IdleState(this),                    // Numéro 0
            new Butcher_StunState(this),                    // Numéro 1
            new Butcher_AlerteState(this),                  // Numéro 2
            new Butcher_ChaseState(this),                   // Numéro 3
            new Butcher_ImpatienceState(this),              // Numéro 4
            new Butcher_AttackState(this),                  // Numéro 5
            new FrozenState(this),                          // Numéro 6
            new DieState(this),                             // Numéro 7
            new VictoryState(this),                         // Numéro 8
		});
        string[] enemyStateNames = System.Enum.GetNames(typeof(EnemyButcherState));
        if (m_sM.States.Count != enemyStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in ZglorgController and EnemyStateEnum");
        }
    }

    public override bool TargetInImpatienceDonuts()
    {
        if (GetTargetDistance(TargetStats1.gameObject.transform) <= rangeMaxForJump && GetTargetDistance(TargetStats1.gameObject.transform) >= rangeMinForJump)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool CheckIfAnimEnded()
    {
        m_tempsJumpAnim -= Time.deltaTime;
        if (m_tempsJumpAnim <= 0)
        {
            return true;
        }
        return false;
    }

    public override bool CoolDownImpatience()
    {
        m_cdImpatient -= Time.deltaTime;
        if(m_cdImpatient <= 0)
        {
            return true;
        }
        return false;
    }

    public override void Timed()
    {
        if (CheckCollision(RedBoxScale, RedBoxPosition) != null)
        {
            TargetStats1.TakeDamage(MyStas.damage.GetValue());
        }
    }

    public override void Attack()
    {
        Anim.SetTrigger("Attack");
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = minRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMinForJump);

        Gizmos.color = maxRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMaxForJump);

    }
}
