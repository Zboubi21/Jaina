using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoThrowSpellState : IState
{
	// Constructor (CTOR)
	PlayerManager m_playerManager;
    public NoThrowSpellState (PlayerManager playerManager){
		  m_playerManager = playerManager;
    }

    public void Enter()
    {

    }

    public void Update()
    {
		if(m_playerManager.m_rightMouseClick){
			m_playerManager.MovePlayer();
		}

		m_playerManager.RotatePlayer();

		if(m_playerManager.m_leftMouseClick || m_playerManager.m_leftMouseDownClick){
			m_playerManager.m_autoAttackCooldown = true;
		}

		if(m_playerManager.m_blinkButton && m_playerManager.m_powers.m_blink.m_canSwitch){
			m_playerManager.ChangeState(1);
		}

		if(m_playerManager.m_iceBlockButton && m_playerManager.m_powers.m_Block.m_canSwitch){
			m_playerManager.ChangeState(2);
		}

		// LEFT SPELLS
		if(m_playerManager.m_leftSpellButton && m_playerManager.m_currentElement == m_playerManager.m_iceElement && m_playerManager.m_powers.m_iceNova.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;
			m_playerManager.ChangeState(3);
		}
		if(m_playerManager.m_leftSpellButton && m_playerManager.m_currentElement == m_playerManager.m_fireElement && m_playerManager.m_powers.m_fireBalls.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;
			m_playerManager.ChangeState(4);
		}
		if(m_playerManager.m_leftSpellButton && m_playerManager.m_currentElement == m_playerManager.m_arcaneElement && m_playerManager.m_powers.m_arcaneProjectiles.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;
			m_playerManager.ChangeState(5);
		}

		// RIGHT SPELLS
		if(m_playerManager.m_rightSpellButton && m_playerManager.m_currentElement == m_playerManager.m_iceElement && m_playerManager.m_powers.m_iceBuff.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;

			if(m_playerManager.m_powers.m_iceBuff.m_actualBuff != null){
				m_playerManager.m_powers.m_iceBuff.m_actualBuff.GetComponent<IceBuff>().DestroyBuff();
			}
			m_playerManager.ChangeState(6);
		}
		if(m_playerManager.m_rightSpellButton && m_playerManager.m_currentElement == m_playerManager.m_fireElement && m_playerManager.m_powers.m_fireTrail.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;
			m_playerManager.ChangeState(7);
		}
		if(m_playerManager.m_rightSpellButton && m_playerManager.m_currentElement == m_playerManager.m_arcaneElement && m_playerManager.m_powers.m_arcaneExplosion.m_canSwitch && m_playerManager.m_canThrowSpell){
			m_playerManager.m_canThrowSpell = false;
			m_playerManager.ChangeState(8);
		}

    }

    public void FixedUpdate()
    {

    }

	public void Exit()
    {

    }

}
