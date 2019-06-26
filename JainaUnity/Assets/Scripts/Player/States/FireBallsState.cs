using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallsState : IState {

	float m_stateTimer = 0;
	bool m_spellIsThrow = false;

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public FireBallsState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter(){
		m_stateTimer = 0;
		m_spellIsThrow = false;
		m_playerManager.StopPlayerMovement();
    }

    public void Update(){
		if(m_stateTimer >= m_playerManager.m_powers.m_fireBalls.m_waitTimeToThrowSpell && !m_spellIsThrow){
			m_spellIsThrow = true;
			m_playerManager.InstantiateSpells(m_playerManager.m_powers.m_fireBalls.m_balls, m_playerManager.m_powers.m_fireBalls.m_root.position, m_playerManager.m_powers.m_fireBalls.m_root.rotation);
		}

		if(m_stateTimer >= m_playerManager.m_powers.m_fireBalls.m_waitTimeToExitState + m_playerManager.m_powers.m_fireBalls.m_waitTimeToThrowSpell){
			m_playerManager.ChangeState(0);
		}

		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.m_powers.m_fireBalls.m_startCooldown = true;
		m_playerManager.MovePlayer();
		m_playerManager.m_canThrowSpell = true;
		m_playerManager.ChangePower(false);			
    }
}
