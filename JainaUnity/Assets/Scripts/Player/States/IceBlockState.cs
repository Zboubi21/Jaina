using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlockState : IState {

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public IceBlockState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter(){
        m_playerManager.m_powers.m_Block.m_inIceBlock = true;

        m_playerManager.StopPlayerMovement();

		m_playerManager.m_powers.m_Block.m_actualIceBlockTimer = 0;

		m_playerManager.m_powers.m_Block.m_block.SetActive(true);
    }

    public void Update(){
		ManageTimer();

		if(m_playerManager.m_iceBlockButton){
			m_playerManager.ChangeState(0);
		}
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.m_powers.m_Block.m_block.SetActive(false);
		m_playerManager.m_powers.m_Block.m_startCooldown = true;
        m_playerManager.m_powers.m_Block.m_inIceBlock = false;
    }

    void ManageTimer(){
		m_playerManager.m_powers.m_Block.m_actualIceBlockTimer += Time.deltaTime;

		if(m_playerManager.m_powers.m_Block.m_actualIceBlockTimer >= m_playerManager.m_powers.m_Block.m_timeToBeInIceBlock){
			m_playerManager.ChangeState(0);
		}
	}

}
