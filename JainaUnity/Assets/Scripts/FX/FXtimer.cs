using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXtimer : MonoBehaviour {

	[SerializeField] private float m_timeToDeleteFX = 1f;

	void Awake(){
		Destroy(gameObject, m_timeToDeleteFX);
	}
	
}
