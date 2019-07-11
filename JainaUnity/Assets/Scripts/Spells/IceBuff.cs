using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBuff : Spell {

	[SerializeField] float m_timeToLive = 8;

	[Header("Scales")]
	[SerializeField] float m_timeToWaitBeforeScale = 1;
	[Space]
	[Header("IceBuff FX scale")]
	[SerializeField] Transform m_FxTransform;
	[SerializeField] float m_speedToScaleFX = 1;
	[SerializeField] float m_iceBuffFXScaleMini;
	[SerializeField] float m_iceBuffFXScaleMaxi;

	[Header("Sphere collider scale")]
	[SerializeField] SphereCollider m_sphereCollider;
	[SerializeField] float m_speedToScaleCollider = 1;
	[SerializeField] float m_sphereColliderScaleMini;
	[SerializeField] float m_sphereColliderScaleMaxi;

	PlayerManager m_playerManager;
	bool m_canModifyTheScale = false;
	Animation m_anim;
	float m_actualFxScale;
	float m_actualColliderScale;
	float m_actualTimer = 0;

	public override void Start(){
        base.Start();
		m_playerManager = PlayerManager.Instance;
		m_anim = GetComponent<Animation>();
	}

	void OnEnable(){
		StartCoroutine(DestroyBuffCoroutine());

		m_actualFxScale = m_iceBuffFXScaleMaxi;
		m_actualColliderScale = m_sphereColliderScaleMaxi;

		m_canModifyTheScale = false;
		m_actualTimer = 0;

		if(m_playerManager != null)
			m_playerManager.m_moveSpeed.m_iceBuffIsCast = true;
	}

	void FixedUpdate(){
		if(m_actualTimer <= m_anim.clip.length){
			m_actualTimer += Time.deltaTime;
		}else{
			m_canModifyTheScale = true;
		}
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

			m_actualFxScale -= Time.deltaTime * m_speedToScaleFX;
			if(m_actualFxScale < m_iceBuffFXScaleMini){
				DestroyBuff();
			}else{
				m_FxTransform.localScale = new Vector3(m_actualFxScale, m_FxTransform.localScale.y, m_actualFxScale);
			}

			m_actualColliderScale -= Time.deltaTime * m_speedToScaleCollider;
			if(m_actualColliderScale > m_sphereColliderScaleMini){
				m_sphereCollider.radius = m_actualColliderScale;
			}

		}
	}

	void OnTriggerExit(Collider col){
		if(col.CompareTag("Player")){
			m_playerManager.m_moveSpeed.m_playerInBuff = false;
			m_playerManager.m_autoAttacks.m_isBuff = false;			
		}
	}

	IEnumerator DestroyBuffCoroutine(){
		yield return new WaitForSeconds(m_timeToLive);
		DestroyBuff();
	}

	public void DestroyBuff(){
		m_playerManager.m_autoAttacks.m_isBuff = false;

		m_playerManager.m_moveSpeed.m_playerInBuff = false;

		ObjectPoolerInstance.ReturnSpellToPool(m_spellType, gameObject);
	}

}
