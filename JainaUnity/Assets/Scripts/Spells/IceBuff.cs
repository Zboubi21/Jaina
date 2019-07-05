using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBuff : Spell {

	[SerializeField] float m_timeToLive = 8;

	[Header("Scales")]
	[SerializeField] float m_timeToWaitBeforeScale = 1;
	[Space]
	[SerializeField] float m_speedToScale = 1;
	[Space]
	[SerializeField] float m_scaleMini;
	[SerializeField] float m_scaleMaxi;

	PlayerManager m_playerManager;
	bool m_canModifyTheScale = false;
	float m_actualScale;
	Animation m_anim;

	void Start(){
		m_playerManager = PlayerManager.Instance;
		StartCoroutine(DestroyBuffCoroutine());
		m_actualScale = m_scaleMaxi;
		m_anim = GetComponent<Animation>();
		StartCoroutine(WaitTheEndOfTheStartAnimation());

		m_playerManager.m_moveSpeed.m_iceBuffIsCast = true;
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			m_playerManager = col.GetComponent<PlayerManager>();
			m_playerManager.m_autoAttacks.m_isBuff = true;
			m_playerManager.On_AutoAttackBuffChange(true);

			m_playerManager.m_moveSpeed.m_iceBuffIsCast = true;
			m_playerManager.m_moveSpeed.m_playerInBuff = true;
			m_playerManager.SetPlayerSpeed(m_playerManager.m_moveSpeed.m_fastSpeed);
		}
	}

	void OnTriggerStay(Collider col){
		if(col.CompareTag("Player")){

			if(!m_canModifyTheScale)
				return;

			m_actualScale -= Time.deltaTime * m_speedToScale;

			if(m_actualScale < m_scaleMini){
				DestroyBuff();
			}else{
				transform.localScale = new Vector3(m_actualScale, transform.localScale.y, m_actualScale);
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_playerManager.m_moveSpeed.m_playerInBuff = false;
			m_playerManager.m_autoAttacks.m_isBuff = false;			
		}
	}

	IEnumerator WaitTheEndOfTheStartAnimation(){
		yield return new WaitForSeconds(m_anim.clip.length);
		m_canModifyTheScale = true;
	}

	IEnumerator DestroyBuffCoroutine(){
		yield return new WaitForSeconds(m_timeToLive);
		DestroyBuff();
	}

	public void DestroyBuff(){
		m_playerManager.m_autoAttacks.m_isBuff = false;

		m_playerManager.m_moveSpeed.m_playerInBuff = false;

		Destroy(gameObject);
	}

}
