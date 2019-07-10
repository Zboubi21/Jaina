using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class ObjectPooler : MonoBehaviour {

#region Singleton

	public static ObjectPooler Instance;

	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of ObjectPooler");
		}
	}

#endregion Singleton

	[Header("Enemy pools")]
	[SerializeField] List<EnemyPool> m_enemyPools;
	[System.Serializable] public class EnemyPool{
        public string m_name;
        public EnemyType m_enemyType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Header("Object pools")]
	[SerializeField] List<ObjectPool> m_objectPools;
	[System.Serializable] public class ObjectPool{
        public string m_name;
        public ObjectType m_objectType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Space]

	[Header("Pool test")]
	[SerializeField] bool m_usePoolTest = true;
	[SerializeField] Transform m_spawnPool;
	[SerializeField] PoolTest[] m_poolTest;
	[System.Serializable] public class PoolTest{
        public KeyCode m_input;
        public EnemyType m_objectToSpawn;
    }

	Dictionary<EnemyType, Queue<GameObject>> m_enemyPoolDictionary;
	Dictionary<ObjectType, Queue<GameObject>> m_objectPoolDictionary;

	void Start(){
		m_enemyPoolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();
		foreach(EnemyPool pool in m_enemyPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			m_enemyPoolDictionary.Add(pool.m_enemyType, objectPool);
		}

		m_objectPoolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();
		foreach(ObjectPool pool in m_objectPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			m_objectPoolDictionary.Add(pool.m_objectType, objectPool);
		}
	}

	void Update(){
		if(m_usePoolTest){
			for (int i = 0, l = m_poolTest.Length; i < l; ++i){
				if(Input.GetKeyDown(m_poolTest[i].m_input)){
					if(m_spawnPool != null){
						SpawnEnemyFromPool(m_poolTest[i].m_objectToSpawn, m_spawnPool.position, m_spawnPool.rotation);
					}else{
						SpawnEnemyFromPool(m_poolTest[i].m_objectToSpawn, Vector3.zero, Quaternion.identity);
					}
				}
			}
		}
    }

	public GameObject SpawnEnemyFromPool(EnemyType enemyType, Vector3 position, Quaternion rotation){

		if(!m_enemyPoolDictionary.ContainsKey(enemyType)){
			Debug.LogWarning("Pool of " + enemyType + " dosen't exist.");
			return null;
		}

		if(m_enemyPoolDictionary[enemyType].Peek().activeSelf){
			Debug.LogError("All " + enemyType + " are already active!");
			return null;
		}

		GameObject objectToSpawn = m_enemyPoolDictionary[enemyType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		return objectToSpawn;
	}
	public void ReturnEnemyToPool(EnemyType enemyType, GameObject objectToReturn){
		objectToReturn.SetActive(false);
		m_enemyPoolDictionary[enemyType].Enqueue(objectToReturn);
	}

	public GameObject SpawnObjectFromPool(ObjectType objectType, Vector3 position, Quaternion rotation){

		if(!m_objectPoolDictionary.ContainsKey(objectType)){
			Debug.LogWarning("Pool of " + objectType + " dosen't exist.");
			return null;
		}

		if(m_objectPoolDictionary[objectType].Peek().activeSelf){
			Debug.LogError("All " + objectType + " are already active!");
			return null;
		}

		GameObject objectToSpawn = m_objectPoolDictionary[objectType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		return objectToSpawn;
	}
	public void ReturnObjectToPool(ObjectType objectType, GameObject objectToReturn){
		objectToReturn.SetActive(false);
		m_objectPoolDictionary[objectType].Enqueue(objectToReturn);
	}

}
