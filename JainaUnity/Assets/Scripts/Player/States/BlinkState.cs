﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class BlinkState : IState {

  Vector3 destination;

	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public BlinkState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter(){
      m_playerManager.m_powers.m_blink.m_startCooldown = true;
      m_playerManager.StopPlayerMovement();	

      m_playerManager.SetBlinkTrailRenderer();
      m_playerManager.m_powers.m_blink.m_blinkFx.Play(true);
      m_playerManager.m_powers.m_blink.m_rightCircleFx.Play(true);
      m_playerManager.m_powers.m_blink.m_leftCircleFx.Play(true);

			m_playerManager.InstantiateGameObject(m_playerManager.m_powers.m_blink.m_spellSound, m_playerManager.transform.position, Quaternion.identity);
    }

    public void Update(){
      if(m_playerManager.m_powers.m_blink.m_useMouseBlink){
        destination = new Vector3(Mathf.Clamp(CameraManager.Instance.m_cursorPosition.x, m_playerManager.transform.position.x - m_playerManager.m_powers.m_blink.m_maxDistance, m_playerManager.transform.position.x + m_playerManager.m_powers.m_blink.m_maxDistance), CameraManager.Instance.m_cursorPosition.y, Mathf.Clamp(CameraManager.Instance.m_cursorPosition.z, m_playerManager.transform.position.z - m_playerManager.m_powers.m_blink.m_maxDistance, m_playerManager.transform.position.z + m_playerManager.m_powers.m_blink.m_maxDistance));
      }else{
        destination = m_playerManager.transform.position + m_playerManager.transform.forward * m_playerManager.m_powers.m_blink.m_maxDistance;
      }
      
      Vector3 origin = m_playerManager.m_powers.m_blink.m_rayCastToCanBlink.position;
      float maxDistance = Vector3.Distance(origin, destination);
      RaycastHit hit;
      if(Physics.Raycast(origin, destination, out hit, maxDistance, m_playerManager.m_powers.m_blink.m_colliderToCanNotBlink)){
        Vector3 newDestination = hit.point;
        destination = new Vector3(newDestination.x, destination.y, newDestination.z);
      }
      // m_playerManager.transform.position = destination;  // IMPOSSIBLE de se TP avec le vide au millieu
      m_playerManager.SetTpPoint(destination);              // POSSIBLE de se TP avec le vide au millieu
      
      m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
    }

    public void FixedUpdate(){

    }

	public void Exit(){
    m_playerManager.PlayerTargetPosition = Vector3.zero;
  }

}
