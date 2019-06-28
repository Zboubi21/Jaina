using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlameInstantiator : MonoBehaviour {

	[SerializeField] GameObject m_fireFlame;
	[SerializeField] int m_objectNb = 10;

	float m_rotation = 360;
	float m_rotationDivise;

	void Awake(){
		m_rotationDivise = m_rotation / m_objectNb;

		for(int i = 0; i < m_objectNb; ++i){
			Instantiate(m_fireFlame, transform.position, Quaternion.Euler(0, m_rotationDivise * i, 0), transform);
		}

	}

	public void DestroyInstantiator(float time){
		// Debug.Log("Je vais mourir dans : " + time);
		Destroy(gameObject, time);
	}

}
