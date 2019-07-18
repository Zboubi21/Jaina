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
    [Space]
    public int impactDamage;
    public float rangeImpact;
    public Color impactRangeColor;
    [Space]
    [Space]
    public float rangeMinForJump;
    public Color minRangeColor;
    [Space]
    public float rangeMaxForJump;
    public Color maxRangeColor;
    [Space]
    public float m_tempsJumpAnim = 2f;

    public ButcherJump m_butcherJump = new ButcherJump();
	[System.Serializable] public class ButcherJump {
		public float m_timeToDoJump = 1;
        public AnimationCurve m_jumpCurve;
	}

    float m_animTime;
    float m_cdImpatient;
    bool isImpatience;

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

    public bool IsImpatience
    {
        get
        {
            return isImpatience;
        }

        set
        {
            isImpatience = value;
        }
    }

    public float AnimTime
    {
        get
        {
            return m_animTime;
        }

        set
        {
            m_animTime = value;
        }
    }
#endregion
    public override void LogicAtStart()
    {
        base.LogicAtStart();

        m_cdImpatient = CoolDownGettingImpatient;
        m_animTime = m_tempsJumpAnim;
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

    public override void OnImpactDamage()
    {
        Collider[] hitCollider = Physics.OverlapSphere(transform.position, rangeImpact);
        for (int i = 0, l = hitCollider.Length; i < l; ++i)
        {
            if (hitCollider[i].CompareTag("Player"))
            {
                hitCollider[i].GetComponent<CharacterStats>().TakeDamage(impactDamage);
                break;
            }
        }
    }

    public override void OnChangeToStunState()
    {
        if (!MyStas.IsDead && !isImpatience)
        {
            ChangeState((int)EnemyButcherState.Butcher_StunState);
        }
    }

    public override void Freeze()
    {
        if (!isImpatience)
        {
            base.Freeze();
        }
    }

    public override void TranslateMove(Transform target)
    {
        transform.Translate(target.localPosition);
    }
    public override void FaceTarget(Transform target)
    {
        Vector3 direction = (target.localPosition - transform.localPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = minRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMinForJump);

        Gizmos.color = maxRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeMaxForJump);

        Gizmos.color = impactRangeColor;
        Gizmos.DrawWireSphere(transform.position, rangeImpact);
    }

    public void Jump(Vector3 fromPos, Vector3 toPos){
        StartCoroutine(JumpCoroutine(fromPos, toPos));
    }
    IEnumerator JumpCoroutine(Vector3 fromPos, Vector3 toPos){

        float distance = Vector3.Distance(fromPos, toPos);
		float moveFracJourney = new float();
		float vitesse = distance / m_butcherJump.m_timeToDoJump;

		while(transform.position != toPos){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transform.position = Vector3.Lerp(fromPos, toPos, m_butcherJump.m_jumpCurve.Evaluate(moveFracJourney));
			yield return null;
		}
    }

}
