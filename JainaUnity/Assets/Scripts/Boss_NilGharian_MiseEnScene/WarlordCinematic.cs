using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;

    NavMeshAgent m_agent;
    Animator m_animator;

    public void On_StartCinematic(){
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();

        m_agent.ResetPath();
        m_agent.SetDestination(m_targetAgentPosition.position);
        m_animator.SetTrigger("Chase");
    }

}
