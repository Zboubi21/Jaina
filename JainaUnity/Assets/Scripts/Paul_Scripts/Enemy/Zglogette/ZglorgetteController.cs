using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;

public class ZglorgetteController : EnemyController
{

    [Header("Nil'Gharian Witch attack variables")]
    public float range;
    public LayerMask layers;
    public GameObject projectil;
    [Header("Nil'Gharian Witch Impatience Variables")]
    public int nombreDeGrandeAttack;
    public GameObject impatiencte_Projectil;
    public float TimeBeforeGettingImpatient = 3f;
    float currentTimeBeforeGettingImpatient;
    float currentTimeBeforeGettingImpatientWhenInAttackRange;
    Ray ray;

    RaycastHit hit;
    private void Awake()
    {
        m_sM.AddStates(new List<IState> {
            new IdleState(this),                        // Numéro 0
            new StunState(this),                        // Numéro 1
            new AlerteState(this),                      // Numéro 2
            new Zglorgette_ChaseState(this),            // Numéro 3
            new Zglorgette_ImpatienceState(this),       // Numéro 4
            new Zglorgette_AttackState(this),           // Numéro 5
            new FrozenState(this),                      // Numéro 6
            new DieState(this),                         // Numéro 7
            new VictoryState(this),                     // Numéro 8
		});
        string[] enemyStateNames = System.Enum.GetNames(typeof(EnemyZglorgetteState));
        if (m_sM.States.Count != enemyStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in ZglorgController and EnemyStateEnum");
        }
    }
    public override void Attack()
    {
        Anim.SetTrigger("Attack");
    }

    public int OnRayCast()
    {
        Vector3 rayTarget = TargetStats1.transform.position - transform.position;
        if (Physics.Linecast(transform.position, TargetStats1.transform.position, out hit, layers))
        {
            float targetDistance = Vector3.Distance(transform.position, TargetStats1.transform.position);
            if (targetDistance > range)
            {
                Debug.DrawRay(transform.position, rayTarget, Color.red);
                return 0;
            }
            else if (hit.collider != TargetStats1.GetComponent<Collider>())
            {
                Debug.DrawRay(transform.position, rayTarget, Color.yellow);
                return 1;

            }
            else
            {
                Debug.DrawRay(transform.position, rayTarget, Color.green);
                return 2;
            }
        }
        return 3;
    }
}
