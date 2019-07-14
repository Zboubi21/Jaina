using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : IState {

    // Constructor (CTOR)
	PlayerManager m_playerManager;
    public PlayerDieState (PlayerManager playerManager){
        m_playerManager = playerManager;
    }

	float m_stateTimer = 0;
	bool m_timerIsReached = false;
    bool m_fadeTimerIsReached = false;

    public void Enter(){
		m_stateTimer = 0;
		m_timerIsReached = false;
		m_playerManager.JainaAnimator.SetTrigger("Die");
        m_playerManager.PlayerIsDead = true;
		m_playerManager.StopPlayerMovement();
        m_playerManager.SaveManager.On_PlayerDie();
    }

    public void Update(){
		
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.JainaAnimator.SetTrigger("Die");
        m_playerManager.PlayerIsDead = false;
	}

}
