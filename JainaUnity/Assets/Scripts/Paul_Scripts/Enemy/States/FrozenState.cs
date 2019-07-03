using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class FrozenState : IState
{
    GameObject freezedObject;

    float m_timeBeingFrozen;
    // CONSTRUCTOR
    EnemyController m_enemyController;
    public FrozenState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        BeingStun(true);
        m_timeBeingFrozen = 3f/*PlayerManager.Instance.m_percentMultiplicateur*/;
        if (freezedObject == null)
        {
            freezedObject = m_enemyController.InstantiateObjects(m_enemyController.m_fxs.m_freezed, m_enemyController.transform.position, m_enemyController.transform.rotation, m_enemyController.transform);
        }
        m_enemyController.Anim.SetTrigger("Idle");
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        m_timeBeingFrozen -= Time.deltaTime;
        if(m_timeBeingFrozen <= 0)
        {
            GetOutOfState();
        }
    }

    public void Exit()
    {
        BeingStun(false);
        m_enemyController.DestroyGameObject(freezedObject);
    }

    public virtual void BeingStun(bool b)
    {
        m_enemyController.StopMoving(b);
    }

    public virtual void GetOutOfState()
    {
        m_enemyController.ChangeState(EnemyState.ChaseState);
    }
}

//bool isFreezAble = false;

//GameObject freezedObject;

//m_enemyController.Anim.SetTrigger("Idle");
//IsFreezAble();
//if(freezedObject == null)
//{
//    freezedObject = m_enemyController.InstantiateObjects(m_enemyController.m_fxs.m_freezed, m_enemyController.transform.position, m_enemyController.transform.rotation, m_enemyController.transform);
//}

//if (isFreezAble)
//{
//    if (m_enemyController.FreezTiming())
//    {
//        GetOutOfState();
//    }
//    if (/*m_enemyController.InAttackRange() /*Stopping Distance*/ /*&&*/ m_enemyController.PlayerInAttackBox()) //GreenBox
//    {
//        Debug.Log("nooop");
//        m_enemyController.ChangeState(EnemyState.AttackState); //Attack
//    }
//}

//public void Exit()
//{
//    if (m_enemyController.FreezTiming())
//    {
//        isFreezAble = false;
//        BeFreezed(false);
//        m_enemyController.DestroyGameObject(freezedObject);
//    }
//}

//public virtual void IsFreezAble()
//{
//    isFreezAble = true;
//    BeFreezed(true);
//}

//public virtual void BeFreezed(bool b)
//{
//    m_enemyController.StopMoving(b);
//}

//public virtual void GetOutOfState()
//{
//    m_enemyController.ChangeState(EnemyState.ChaseState);
//}