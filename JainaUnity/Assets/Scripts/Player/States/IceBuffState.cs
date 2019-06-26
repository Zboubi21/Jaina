using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBuffState : IState {
	
	float m_stateTimer = 0;
	bool m_spellIsThrow = false;

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public IceBuffState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter(){
		m_stateTimer = 0;
		m_spellIsThrow = false;
		m_playerManager.StopPlayerMovement();
    }

    public void Update(){
		if(m_stateTimer >= m_playerManager.m_powers.m_iceBuff.m_waitTimeToThrowSpell && !m_spellIsThrow){
			m_spellIsThrow = true;
			GameObject go = m_playerManager.InstantiateSpells(m_playerManager.m_powers.m_iceBuff.m_buff, m_playerManager.m_powers.m_iceBuff.m_root.position, m_playerManager.m_powers.m_iceBuff.m_root.rotation);
			m_playerManager.m_powers.m_iceBuff.m_actualBuff = go;
		}

		if(m_stateTimer >= m_playerManager.m_powers.m_iceBuff.m_waitTimeToExitState + m_playerManager.m_powers.m_iceBuff.m_waitTimeToThrowSpell){
			m_playerManager.ChangeState(0);
		}

		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.m_powers.m_iceBuff.m_startCooldown = true;
		if(!m_playerManager.m_powers.m_iceBuff.m_stopMovingAfterSpell){
			m_playerManager.MovePlayer();
		}
		m_playerManager.m_canThrowSpell = true;
		m_playerManager.ChangePower(true);			
	}

}
