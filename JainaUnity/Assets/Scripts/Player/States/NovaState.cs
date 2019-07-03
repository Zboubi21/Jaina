using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class NovaState : IState {

	float m_stateTimer = 0;
	bool m_spellIsThrow = false;

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public NovaState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter(){
			m_stateTimer = 0;
			m_spellIsThrow = false;
			m_playerManager.StopPlayerMovement();
		m_playerManager.ChangePower(false);			
    }

    public void Update(){
		if(m_stateTimer >= m_playerManager.m_powers.m_iceNova.m_waitTimeToThrowSpell && !m_spellIsThrow){
			m_spellIsThrow = true;
			m_playerManager.InstantiateSpells(m_playerManager.m_powers.m_iceNova.m_nova, m_playerManager.m_powers.m_iceNova.m_root.position, m_playerManager.m_powers.m_iceNova.m_root.rotation);
		}

		if(m_stateTimer >= m_playerManager.m_powers.m_iceNova.m_waitTimeToExitState + m_playerManager.m_powers.m_iceNova.m_waitTimeToThrowSpell){
			m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
		}

		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.m_powers.m_iceNova.m_startCooldown = true;
		m_playerManager.MovePlayer();
		m_playerManager.m_canThrowSpell = true;
		// m_playerManager.ChangePower(false);			
    }
}
