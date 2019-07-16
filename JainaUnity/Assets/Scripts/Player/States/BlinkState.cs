using System.Collections;
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

      RayCast();
    }

    void RayCast(){
      if(m_playerManager.m_powers.m_blink.m_useMouseBlink){
        destination = new Vector3(Mathf.Clamp(CameraManager.Instance.m_cursorPosition.x, m_playerManager.transform.position.x - m_playerManager.m_powers.m_blink.m_maxDistance, m_playerManager.transform.position.x + m_playerManager.m_powers.m_blink.m_maxDistance), CameraManager.Instance.m_cursorPosition.y, Mathf.Clamp(CameraManager.Instance.m_cursorPosition.z, m_playerManager.transform.position.z - m_playerManager.m_powers.m_blink.m_maxDistance, m_playerManager.transform.position.z + m_playerManager.m_powers.m_blink.m_maxDistance));
      }else{
        destination = m_playerManager.transform.position + m_playerManager.transform.forward * m_playerManager.m_powers.m_blink.m_maxDistance;
      }
      destination = new Vector3(destination.x, m_playerManager.m_powers.m_blink.m_rayCastToCanBlink.position.y, destination.z);
      
      Vector3 origin = m_playerManager.m_powers.m_blink.m_rayCastToCanBlink.position;

      // m_playerManager.m_playerDebug.m_fromPos.position = origin; //
      // m_playerManager.m_playerDebug.m_fromPos.LookAt(destination); //

      // m_playerManager.m_playerDebug.m_toPos.position = destination; //
      // m_playerManager.m_playerDebug.m_toPos.LookAt(origin); //

      RaycastHit hit;
      if(Physics.Linecast(origin, destination, out hit, m_playerManager.m_powers.m_blink.m_colliderToCanNotBlink)){
        Debug.DrawRay(origin, destination, Color.black, 5);
        Vector3 newDestination = hit.point;
        destination = new Vector3(newDestination.x, destination.y, newDestination.z);
        // Debug.Log("I hit:" + hit.collider.gameObject.name);
      }

      // m_playerManager.m_playerDebug.m_newToPos.position = destination; //
      // m_playerManager.m_playerDebug.m_newToPos.LookAt(origin); //

      // m_playerManager.transform.position = destination;  // IMPOSSIBLE de se TP avec le vide au millieu
      m_playerManager.SetTpPoint(destination);              // POSSIBLE de se TP avec le vide au millieu
      
      m_playerManager.ChangeState(PlayerState.NoThrowSpellState);
    }

    public void Update(){
      
    }

    public void FixedUpdate(){

    }

	public void Exit(){
    m_playerManager.PlayerTargetPosition = Vector3.zero;
  }

}
