using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class IceBlockState : IState {

	// Constructor (CTOR)
	PlayerManager m_playerManager;
	public IceBlockState (PlayerManager playerManager){
		m_playerManager = playerManager;
	}

	FX m_chronoShield;

	public void Enter(){
		m_playerManager.JainaAnimator.SetBool("ChronoShield", true);
		m_playerManager.m_powers.m_Block.m_inIceBlock = true;
		m_playerManager.StopPlayerMovement();
		m_playerManager.m_powers.m_Block.m_actualIceBlockTimer = 0;
		m_playerManager.m_powers.m_Block.m_block.Play(true);
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
		m_playerManager.m_powers.m_Block.m_startCooldown = true;
		m_playerManager.m_powers.m_Block.m_inIceBlock = false;
		m_playerManager.JainaAnimator.SetBool("ChronoShield", false);
		m_playerManager.m_powers.m_Block.m_block.Stop(true);
		m_playerManager.m_powers.m_Block.m_block.Clear(true);
	}

    void ManageTimer(){
		m_playerManager.m_powers.m_Block.m_actualIceBlockTimer += Time.deltaTime;

		if(m_playerManager.m_powers.m_Block.m_actualIceBlockTimer >= m_playerManager.m_powers.m_Block.m_timeToBeInIceBlock){
			m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
		}
	}

}
