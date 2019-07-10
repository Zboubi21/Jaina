using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class AmbushPoint : MonoBehaviour {

	public EnemyType m_enemyType;

	[Header("Enemies to spawn")]
	[SerializeField] GameObject[] m_spawnEnemies;
	[SerializeField] int[] m_spawnNumbers;
	
	[Header("Gizmos")]
	[SerializeField] Color m_gizmosColor = Color.magenta;

	public GameObject[] m_event;

	void OnDrawGizmosSelected(){
		Gizmos.color = m_gizmosColor;
		Gizmos.DrawSphere(transform.position, 1);
	}

	public void StartAmbush(){
		for(int i = 0, l = m_spawnEnemies.Length; i < l; ++i){

			for(int i2 = 0, l2 = m_spawnNumbers[i]; i2 < l2; ++i2){
				// Instantiate(m_spawnEnemies[i], transform.position, transform.rotation);
				ObjectPooler.Instance.SpawnEnemyFromPool(m_enemyType, transform.position, transform.rotation);
			}
		}
		StartCoroutine(SpawnEnemies());
	}

	IEnumerator SpawnEnemies(){
		for(int i = 0, l = m_event.Length; i < l; ++i){
			m_event[i].SetActive(true);
			print("coroutine appelé");
			// yield return null;
			yield return new WaitForSeconds(0.001f);
			// yield return new WaitForEndOfFrame();
		}
	}

	void Start(){
		StartCoroutine(StartCorout());
	}

	IEnumerator StartCorout(){
		yield return new WaitForSeconds(0.1f);
		for(int i = 0, l = m_event.Length; i < l; ++i){
			m_event[i].SetActive(false);
		}
	}

}
