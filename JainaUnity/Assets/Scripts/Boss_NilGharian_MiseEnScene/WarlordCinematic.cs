using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;
    [SerializeField] float m_timeBeforeFleeing;

    NavMeshAgent m_agent;
    Animator m_animator;

    public void On_StartCinematic(){
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();

        m_agent.ResetPath();

        StartCoroutine(WaitBeforeFleeing(m_timeBeforeFleeing));
    }

    IEnumerator WaitBeforeFleeing(float time)
    {
        yield return new WaitForSeconds(time);
        m_agent.SetDestination(m_targetAgentPosition.position);
        m_animator.SetTrigger("Chase");
    }

}
