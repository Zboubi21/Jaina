using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class FireFlameInstantiator : MonoBehaviour {

	[SerializeField] int m_objectNb = 10;

	PlayerManager m_playerManager;
	float m_rotation = 360;
	float m_rotationDivise;

	void Awake(){
		m_playerManager = PlayerManager.Instance;

		m_rotationDivise = m_rotation / m_objectNb;

		for(int i = 0; i < m_objectNb; ++i){
			FireProjectiles projectile = m_playerManager.ObjectPooler.SpawnSpellFromPool(SpellType.FireBalls, transform.position, m_playerManager.m_playerMesh.transform.rotation * Quaternion.Euler(0, m_rotationDivise * i, 0)).GetComponent<FireProjectiles>();
			if(projectile != null){
				projectile.Ffi = this;
			}
		}

	}

	public void DestroyInstantiator(float time){
		// Debug.Log("Je vais mourir dans : " + time);
		Destroy(gameObject, time);
	}

}
