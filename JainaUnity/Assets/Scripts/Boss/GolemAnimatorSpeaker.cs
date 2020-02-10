using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimatorSpeaker : MonoBehaviour
{
    GolemController m_golemController;
    Collider m_golemCollider;

    void Start()
    {
        m_golemController = GetComponentInParent<GolemController>();
        m_golemCollider = m_golemController.GetComponent<Collider>();
    }

    public void EnableGolemCollider()
    {
        SetGolemCollider(true);
    }
    public void DisableGolemCollider()
    {
        SetGolemCollider(false);
    }
    void SetGolemCollider(bool enable)
    {
        m_golemCollider.enabled = enable;
    }

}
