using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum_Zglorgette;
using PoolTypes;

public class ZglorgetteController : EnemyController
{
    [Space]
    [Header("Nil'Gharian Witch attack variables")]
    public float range;
    public Color rangeColor = Color.cyan;
    public LayerMask layers;
    public SpellType projectil = SpellType.AutoAttack_Zglorgette;
    public Transform projectilRoot;
    [Space]
    [Header("Nil'Gharian Witch Impatience Variables")]
    public int nombreDeGrandeAttack;
    public float timeBetweenImpatiencePorjectil=0.5f;
    public SpellType impatience_Projectil = SpellType.Spell_Zglorgette;
    public Transform impatienceProjectilRoot;
    public float TimeBeforeZglorgetteGettingImpatient = 3f;
    float currentTimeBeforeZglorgetteGettingImpatient;
    Ray ray;

    RaycastHit hit;
    RaycastHit hitD;
    RaycastHit hitG;

    #region get set
    public float CurrentTimeBeforeZglorgetteGettingImpatient
    {
        get
        {
            return currentTimeBeforeZglorgetteGettingImpatient;
        }

        set
        {
            currentTimeBeforeZglorgetteGettingImpatient = value;
        }
    }
    #endregion

    private void Awake()
    {
        m_sM.AddStates(new List<IState> {
            new IdleState(this),                        // Numéro 0
            new Zglorgette_StunState(this),             // Numéro 1
            new AlerteState(this),                      // Numéro 2
            new Zglorgette_ChaseState(this),            // Numéro 3
            new Zglorgette_ImpatienceState(this),       // Numéro 4
            new Zglorgette_AttackState(this),           // Numéro 5
            new FrozenState(this),                      // Numéro 6
            new DieState(this),                         // Numéro 7
            new VictoryState(this),                     // Numéro 8
            new CinematicState(this),                   // Numéro 9

		});
        string[] enemyStateNames = System.Enum.GetNames(typeof(EnemyZglorgetteState));
        if (m_sM.States.Count != enemyStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in ZglorgController and EnemyStateEnum");
        }
    }

    public override void LogicAtStart()
    {
        base.LogicAtStart();
        currentTimeBeforeZglorgetteGettingImpatient = TimeBeforeZglorgetteGettingImpatient;
    }

    public override void Attack()
    {
        Anim.SetTrigger("Attack");
    }

    public override void OnCastProjectil()
    {
        // Instantiate(projectil, projectilRoot);
        ObjectPooler.SpawnSpellFromPool(projectil, projectilRoot.position, projectilRoot.rotation);
    }

    public override void OnCastImpatienceProjectil()
    {
        // Instantiate(impatience_Projectil, impatienceProjectilRoot);
        ObjectPooler.SpawnSpellFromPool(impatience_Projectil, impatienceProjectilRoot.position, impatienceProjectilRoot.rotation);
    }

    public int OnRayCastSide()
    {
        if (GetTargetDistance(Target.transform) > Agent.stoppingDistance)
        {
            float targetDistance = Vector3.Distance(transform.position, TargetStats1.transform.position);

            if (Physics.Raycast((transform.forward + transform.right) + transform.position, transform.forward * range, out hitD, range, layers) || Physics.Raycast((transform.forward - transform.right) + transform.position, transform.forward * range, out hitG, range, layers))
            {

                if (targetDistance > range)
                {
                    //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.red);
                    //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.red);
                    return 0;
                }
                else if (hitD.collider != null && hitG.collider == null)
                {
                    //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.yellow);
                    return 3;

                }
                else if (hitG.collider != null && hitD.collider == null)
                {
                    //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.yellow);
                    return 4;

                }
                else if(hitG.collider != null && hitD.collider != null)
                {
                    //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.yellow);
                    //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.yellow);
                    return 1;
                }
                return 0;
            }
            else 
            {
                if (targetDistance < range)
                {
                    //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.green);
                    //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.green);
                    return 2;
                }
                else
                {
                    //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.red);
                    //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.red);
                    return 0;
                }
            }
        }
        else
        {
            //Debug.DrawRay((transform.forward + transform.right) + transform.position, transform.forward * range, Color.green);
            //Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.green);
            return 2;
        }
    }
    public int OnRayCast()
    {
        Vector3 rayTarget = TargetStats1.transform.position - transform.position;

        if (GetTargetDistance(Target.transform) > Agent.stoppingDistance)
        {
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
                    Collider[] hitcol = Physics.OverlapSphere(transform.position, 2f, layers);
                    if (hitcol.Length == 0)
                    {
                        Debug.DrawRay(transform.position, rayTarget, Color.green);
                        return 2;
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, rayTarget, Color.white);
                        return 3;
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayTarget, Color.magenta);
                return 0;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, rayTarget, Color.green);
            return 2;
        }
    }
    public int OnRayLeftCast()
    {
        if (GetTargetDistance(Target.transform) > Agent.stoppingDistance)
        {
            if (Physics.Raycast((transform.forward - transform.right) + transform.position, transform.forward * range, out hitG, range, layers))
            {
                Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.yellow);

                Debug.Log(hitG.collider);
                return 1;
            }
            else
            {
                float targetDistance = Vector3.Distance(transform.position, TargetStats1.transform.position);

                if (targetDistance > range)
                {

                    Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.red);

                    return 0;
                }
                else
                {
                    Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.green);
                    return 2;
                }
            }
        }
        else
        {
            Debug.DrawRay((transform.forward - transform.right) + transform.position, transform.forward * range, Color.green);

            return 2;
        }
    }


    public override bool CoolDownWitchImpatience()
    {
        TimeBeforeZglorgetteGettingImpatient -= Time.deltaTime;
        if(TimeBeforeZglorgetteGettingImpatient <= 0)
        {
            return true;
        }
        return false;
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
