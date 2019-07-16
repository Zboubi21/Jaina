﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;
using PoolTypes;

public class ArcaneProjectilesState : IState {

	float m_stateTimer = 0;
	bool m_firstSpellIsThrow = false;
	bool m_secondSpellIsThrow = false;
	bool m_thirdSpellIsThrow = false;

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public ArcaneProjectilesState (PlayerManager playerManager){
		m_playerManager = playerManager;
    }

    public void Enter(){
		m_playerManager.JainaAnimator.SetBool("ArcaneProjectiles", true);
		m_stateTimer = 0;
		m_firstSpellIsThrow = false;
		m_secondSpellIsThrow = false;
		m_thirdSpellIsThrow = false;
		m_playerManager.StopPlayerMovement();
		// m_playerManager.ChangePower(false);	
		m_playerManager.StartDecreaseArcaneProjectilesTimer();		
    }	

    public void Update(){
		//First spell
		if(m_stateTimer >= m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowFirstSpell && !m_firstSpellIsThrow){
			m_firstSpellIsThrow = true;
			m_playerManager.ObjectPooler.SpawnSpellFromPool(SpellType.ArcaneProjectile1, m_playerManager.m_powers.m_arcaneProjectiles.m_root.position, m_playerManager.m_powers.m_arcaneProjectiles.m_root.rotation);
			m_playerManager.InstantiateGameObject(m_playerManager.m_powers.m_arcaneProjectiles.m_firstSpellSound, m_playerManager.transform.position, Quaternion.identity);

			if(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_firstShake.m_useShakeCam){
				m_playerManager.ShakeCamera(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_firstShake.m_magnitudeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_firstShake.m_roughnessShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_firstShake.m_fadeInTimeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_firstShake.m_fadeOutTimeShake);
			}

		}

		//Second spell
		if(m_stateTimer >= m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowFirstSpell + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowSecondSpell && !m_secondSpellIsThrow){
			m_secondSpellIsThrow = true;
			m_playerManager.ObjectPooler.SpawnSpellFromPool(SpellType.ArcaneProjectile2, m_playerManager.m_powers.m_arcaneProjectiles.m_root.position, m_playerManager.m_powers.m_arcaneProjectiles.m_root.rotation);
			m_playerManager.InstantiateGameObject(m_playerManager.m_powers.m_arcaneProjectiles.m_secondSpellSound, m_playerManager.transform.position, Quaternion.identity);
		
			if(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_secoundShake.m_useShakeCam){
				m_playerManager.ShakeCamera(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_secoundShake.m_magnitudeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_secoundShake.m_roughnessShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_secoundShake.m_fadeInTimeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_secoundShake.m_fadeOutTimeShake);
			}

		}

		//Third spell
		if(m_stateTimer >= m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowFirstSpell + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowSecondSpell + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowThirdSpell && !m_thirdSpellIsThrow){
			m_thirdSpellIsThrow = true;
			m_playerManager.ObjectPooler.SpawnSpellFromPool(SpellType.ArcaneProjectile3, m_playerManager.m_powers.m_arcaneProjectiles.m_root.position, m_playerManager.m_powers.m_arcaneProjectiles.m_root.rotation);
			m_playerManager.InstantiateGameObject(m_playerManager.m_powers.m_arcaneProjectiles.m_thirdSpellSound, m_playerManager.transform.position, Quaternion.identity);
		
			if(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_thirdShake.m_useShakeCam){
				m_playerManager.ShakeCamera(m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_thirdShake.m_magnitudeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_thirdShake.m_roughnessShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_thirdShake.m_fadeInTimeShake, m_playerManager.m_powers.m_arcaneProjectiles.m_ShakeCamera.m_thirdShake.m_fadeOutTimeShake);
			}
			m_playerManager.ChangePower(false);			
		}

		//On sort de la state
		if(m_stateTimer >= m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToExitState + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowFirstSpell + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowSecondSpell + m_playerManager.m_powers.m_arcaneProjectiles.m_waitTimeToThrowThirdSpell){
			m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
		}

		//Si le joueur veut ressortir de la state
		if(m_playerManager.m_leftSpellButton){
			m_playerManager.ChangePower(false);			
			m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
		}

		m_stateTimer += Time.deltaTime;
    }

    public void FixedUpdate(){

    }

	public void Exit(){
		m_playerManager.m_powers.m_arcaneProjectiles.m_startCooldown = true;
		m_playerManager.MovePlayer();
		m_playerManager.m_canThrowSpell = true;
		// m_playerManager.ChangePower(false);			
		m_playerManager.JainaAnimator.SetBool("ArcaneProjectiles", false);
		m_playerManager.StopDecreaseArcaneProjectilesTimer();
    }
	
}
