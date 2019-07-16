using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;

    NavMeshAgent m_agent;

    void OnEnable(){
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.SetDestination(m_targetAgentPosition.position);
    }

}
