using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemyLifeBarManager : MonoBehaviour {

	public static BigEnemyLifeBarManager Instance;

	[SerializeField] Transform m_bigRoot;

	EnemyStats m_enemyStats;

	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of BigEnemyLifeBarManager");
		}
	}

	public void ShowLifeBar(EnemyStats enemyStat){
		m_enemyStats = enemyStat;
	}

}
