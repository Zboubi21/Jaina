using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFXWithEvent : MonoBehaviour {

	[SerializeField] float m_timeToSpawn = 8;

	public void AddSound(GameObject go){
		Level.AddFX(go, Vector3.zero, Quaternion.identity);
	}

	public void AddSoundWithTime(GameObject go){
		StartCoroutine(WaitToSpawn(go));
	}

	IEnumerator WaitToSpawn(GameObject go){
		yield return new WaitForSeconds(m_timeToSpawn);
		Level.AddFX(go, Vector3.zero, Quaternion.identity);
	}

}

