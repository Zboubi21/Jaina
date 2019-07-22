using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarlordCinematic : MonoBehaviour {
    
    [SerializeField] Transform m_targetAgentPosition;
    [SerializeField] float m_timeBeforeFleeing;
    [SerializeField] float m_timeBeforeDisapering = 2.5f;

    public Animation AnimationToPlayeAtEndOfCinematique = Animation.Impatience;
    [System.Serializable]
    public enum Animation
    {
        Idle,
        Chase,
        Attack,
        Impatience,
        Hit,
        Die,
        Alerte,
        Victory,
    }

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
        switch (AnimationToPlayeAtEndOfCinematique)
        {
            case Animation.Idle:
                m_animator.SetTrigger("Idle");
                break;
            case Animation.Chase:
                m_animator.SetTrigger("Chase");
                break;
            case Animation.Attack:
                m_animator.SetTrigger("Attack");
                break;
            case Animation.Impatience:
                m_animator.SetTrigger("Impatience");
                break;
            case Animation.Hit:
                m_animator.SetTrigger("Hit");
                break;
            case Animation.Die:
                m_animator.SetTrigger("Die");
                break;
            case Animation.Alerte:
                m_animator.SetTrigger("Alerte");
                break;
            case Animation.Victory:
                m_animator.SetTrigger("Victory");
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(timeoff);   // Disable to use EnemyBecameInvisible
        gameObject.SetActive(false);                // Disable to use EnemyBecameInvisible
    }
}
