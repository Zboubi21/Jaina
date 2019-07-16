using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCinematicState : IState {

    // Constructor (CTOR)
	PlayerManager m_playerManager;
    public PlayerCinematicState (PlayerManager playerManager){
        m_playerManager = playerManager;
    }

    float m_stateTimer = 0;
    float m_timeToBeInCinematic = 0;
    bool m_timerIsReached = false;

    public void Enter(){
        m_stateTimer = 0;
        m_timeToBeInCinematic = m_playerManager.m_cinematic.m_timeToBeInCinematic;
        m_timerIsReached = false;
        // m_playerManager.JainaAnimator.SetBool("isMoving", false);
        m_playerManager.m_cinematic.m_isInCinematicState = true;
        m_playerManager.StartCinematicStringCorout(true);
		m_playerManager.StopPlayerMovement();
    }

    public void Update(){
		if(m_stateTimer >= m_timeToBeInCinematic && !m_timerIsReached){
			m_timerIsReached = true;
		}
		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
        m_playerManager.m_cinematic.m_isInCinematicState = false;
        m_playerManager.StartCinematicStringCorout(false);
	}

}
