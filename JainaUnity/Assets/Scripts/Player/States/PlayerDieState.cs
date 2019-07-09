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

    public void Enter(){
		m_stateTimer = 0;
		m_timerIsReached = false;
		m_playerManager.JainaAnimator.SetBool("Die", true);
        m_playerManager.PlayerIsDead = true;
    }

    public void Update(){
		if(m_stateTimer >= m_playerManager.m_death.m_timeToRespawn && !m_timerIsReached){
			m_timerIsReached = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}

		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.JainaAnimator.SetBool("Die", false);
        m_playerManager.PlayerIsDead = false;
	}

}
