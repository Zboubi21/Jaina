using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;

public class ZglorgController : EnemyController {


    [Header("Nil'Gharian Impatience Variables")]
    public float speedSprint = 15f;
    public float TimeBeforeGettingImpatient = 3f;
    float currentTimeBeforeGettingImpatient;
    float currentTimeBeforeGettingImpatientWhenInAttackRange;
    float attackImpatience = 3f;
    float currentImpatience;
    /*bool beingAttacked;
    bool hasBeenAttacked;
    bool isStun;*/

    public override void LogicAtStart()
    {
        base.LogicAtStart();
        currentTimeBeforeGettingImpatient = TimeBeforeGettingImpatient;
        currentTimeBeforeGettingImpatientWhenInAttackRange = TimeBeforeGettingImpatient;
        currentImpatience = attackImpatience;
    }

    private void Awake()
    {
        m_sM.AddStates(new List<IState> {
            new IdleState(this),            // Numéro 0
            new StunState(this),            // Numéro 1
            new AlerteState(this),          // Numéro 2
            new ChaseState(this),           // Numéro 3
            new ImpatienceState(this),      // Numéro 4
            new AttackState(this),          // Numéro 5
            new FrozenState(this),          // Numéro 6
            new DieState(this),             // Numéro 7
            new VictoryState(this),         // Numéro 8
            new CinematicState(this),       // Numéro 9
		});
        string[] enemyStateNames = System.Enum.GetNames (typeof(EnemyState));
		if(m_sM.States.Count != enemyStateNames.Length){
			Debug.LogError("You need to have the same number of State in ZglorgController and EnemyStateEnum");
		}
    }

    public override void Timed()
    {
        if(CheckCollision(RedBoxScale,RedBoxPosition) != null)
        {
            TargetStats1.TakeDamage(MyStas.damage.GetValue());
        }
    }

    public override void Sprint(float speed)
    {
        float newSpeed = speed * Enemystats.Slow;
        Agent.speed = newSpeed;
    }
    public override bool IsChasing()
    {
        currentTimeBeforeGettingImpatient -= Time.deltaTime;
        if (currentTimeBeforeGettingImpatient <= 0)
        {
            ImpatienceSign.gameObject.SetActive(true);
            ImpatienceSign.StartParticle();
            currentTimeBeforeGettingImpatient = TimeBeforeGettingImpatient;
            return true;
        }
        return false;
    }
    public override bool IsInAttackRangeForToLong()
    {
        currentTimeBeforeGettingImpatientWhenInAttackRange -= Time.deltaTime;
        if (currentTimeBeforeGettingImpatientWhenInAttackRange <= 0)
        {
            currentTimeBeforeGettingImpatientWhenInAttackRange = TimeBeforeGettingImpatient;
            return true;
        }
        return false;
    }
    public override bool CanAttackWhenImpatience()
    {
        currentImpatience -= Time.deltaTime;
        if (currentImpatience <= 0)
        {
            ImpatienceSign.gameObject.SetActive(true);
            ImpatienceSign.StartParticle();
            currentImpatience = attackImpatience;
            return true;
        }
        else if (!PlayerInAttackBox())
        {
            return false;
        }
        return false;
    }

    public override void Attack()
    {
        Anim.SetTrigger("Attack");
    }

    public override void IceSlow()
    {
        
    }

}


//////////////////////////////////////////////////////////////////////////////////////////////////////
/// SAUVEGARDE DU 18/04/2019 (chez Paul) AVANT DE METTRE EN PLACE LA STATE MACHINE POUR LES ENEMIS ///
//////////////////////////////////////////////////////////////////////////////////////////////////////
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZglorgController : EnemyController {

    float m_time;

    CharacterStats m_target;

    public override void Start()
    {
        base.Start();
        m_target = PlayerManager.Instance.gameObject.transform.GetComponent<CharacterStats>();
    }

    public override void Attack(CharacterStats targetStats)
    {
        base.Attack(targetStats);
        StartCoroutine(DoDamage(targetStats, Rac.animationClips[0].length));
    }


    IEnumerator DoDamage(CharacterStats stats, float delay)
    {
        yield return new WaitForSeconds(delay);
        AnimEnd1 = false;
    }

    public override void Timed()
    {
        m_target.TakeDamage(MyStas.damage.GetValue());
    }
}*/
//////////////////////////////////////////////////////////////////////////////////////////////////////
/// SAUVEGARDE DU 18/04/2019 (chez Paul) AVANT DE METTRE EN PLACE LA STATE MACHINE POUR LES ENEMIS ///
//////////////////////////////////////////////////////////////////////////////////////////////////////
