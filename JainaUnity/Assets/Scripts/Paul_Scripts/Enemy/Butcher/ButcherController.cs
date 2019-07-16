using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
public class ButcherController : EnemyController
{
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
		});
        string[] enemyStateNames = System.Enum.GetNames(typeof(EnemyState));
        if (m_sM.States.Count != enemyStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in ZglorgController and EnemyStateEnum");
        }
    }

    public override void Timed()
    {
        if (CheckCollision(RedBoxScale, RedBoxPosition) != null)
        {
            TargetStats1.TakeDamage(MyStas.damage.GetValue());
        }
    }
}
