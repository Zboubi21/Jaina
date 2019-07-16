using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;

    NavMeshAgent m_agent;
    Animator m_animator;

    void Start(){
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();
    }

    void OnEnable(){
        m_agent.SetDestination(m_targetAgentPosition.position);
        m_animator.SetTrigger("Chase");
    }

}
