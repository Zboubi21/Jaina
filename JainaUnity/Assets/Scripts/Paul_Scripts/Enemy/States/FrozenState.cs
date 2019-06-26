using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenState : IState
{
    float FreezTime;
    bool isFreezAble = false;

    GameObject freezedObject;

    // CONSTRUCTOR
    EnemyController m_enemyController;
    public FrozenState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        IsFreezAble();
        freezedObject = m_enemyController.InstantiateObjects(m_enemyController.m_fxs.m_freezed, m_enemyController.transform.position, m_enemyController.transform.rotation, m_enemyController.transform);
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        if (isFreezAble)
        {
            FreezTime -= Time.deltaTime;
            if(FreezTime <= 0)
            {
                GetOutOfState();
            }
        }
    }

    public void Exit()
    {
        BeFreezed(false);
        m_enemyController.DestroyGameObject(freezedObject);
    }

    public virtual void IsFreezAble()
    {
        isFreezAble = true;
        BeFreezed(true);
        FreezTime = PlayerManager.Instance.m_powers.m_iceNova.m_timeFreezed;
    }

    public virtual void BeFreezed(bool b)
    {
        m_enemyController.StopMoving(b);
    }

    public virtual void GetOutOfState()
    {
        m_enemyController.ChangeState(3);
        /*if (m_enemyController.GetLastStateIndex() != 1)
        {
            m_enemyController.ChangeState(m_enemyController.GetLastStateIndex());
        }
        else
        {
            
        }*/
    }

}
