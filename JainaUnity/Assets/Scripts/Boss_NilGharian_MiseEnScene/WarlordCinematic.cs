using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;
    [SerializeField] float m_timeBeforeFleeing;
    [SerializeField] float m_timeBeforeDisapering = 2.5f;

    NavMeshAgent m_agent;
    Animator m_animator;

    public void On_StartCinematic(){
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();

        m_agent.ResetPath();

        StartCoroutine(WaitBeforeFleeing(m_timeBeforeFleeing, m_timeBeforeDisapering));
    }

    IEnumerator WaitBeforeFleeing(float time, float timeoff)
    {
        m_animator.SetTrigger("Hit");
        yield return new WaitForSeconds(time);
        m_agent.SetDestination(m_targetAgentPosition.position);
        m_agent.speed += 5;
        m_animator.SetTrigger("Impatience");
        yield return new WaitForSeconds(timeoff);
        gameObject.SetActive(false);
    }
}
