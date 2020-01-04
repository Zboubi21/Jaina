using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    GolemController m_golemController;
    public GolemController GolemController { get { return m_golemController; } set { m_golemController = value; } }

    public virtual void On_AttackBegin(int phaseNbr)
    {
        // Debug.Log("On_AttackBegin: " + this);
    }
    public virtual void On_AttackEnd()
    {
        // Debug.Log("On_AttackEnd: " + this);
        if(GolemController != null)
        {
            GolemController.On_AttackIsFinished();
        }
    }
}
