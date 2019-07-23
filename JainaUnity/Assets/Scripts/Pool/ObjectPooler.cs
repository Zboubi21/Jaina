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
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of ObjectPooler");
			gameObject.SetActive(false);
            Destroy(gameObject);
		}
	}

#endregion Singleton

	[Header("Enemy pools")]
	[SerializeField] List<EnemyPool> m_enemyPools;
	[System.Serializable] public class EnemyPool {
        public string m_name;
        public EnemyType m_enemyType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Header("Spell pools")]
	[SerializeField] List<SpellPool> m_spellPools;
	[System.Serializable] public class SpellPool {
        public string m_name;
        public SpellType m_spellType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Header("Object pools")]
	[SerializeField] List<ObjectPool> m_objectPools;
	[System.Serializable] public class ObjectPool {
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

	[SerializeField] ObjectPoolTest[] m_objectPoolTest;
	[System.Serializable] public class ObjectPoolTest{
        public KeyCode m_input;
        public ObjectType m_objectToSpawn;
    }

	Dictionary<EnemyType, Queue<GameObject>> m_enemyPoolDictionary;
	Dictionary<SpellType, Queue<GameObject>> m_spellPoolDictionary;
	Dictionary<ObjectType, Queue<GameObject>> m_objectPoolDictionary;

	Queue<PoolTracker> m_trackedObject = new Queue<PoolTracker>();
	Queue<PoolTracker> m_lifePotionTracked = new Queue<PoolTracker>();

	void Start(){
		m_enemyPoolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();
		foreach(EnemyPool pool in m_enemyPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
				objectPool.Enqueue(obj);
			}
			m_enemyPoolDictionary.Add(pool.m_enemyType, objectPool);
		}

		m_spellPoolDictionary = new Dictionary<SpellType, Queue<GameObject>>();
		foreach(SpellPool pool in m_spellPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
				objectPool.Enqueue(obj);
			}
			m_spellPoolDictionary.Add(pool.m_spellType, objectPool);
		}

		m_objectPoolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();
		foreach(ObjectPool pool in m_objectPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
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
			for (int i = 0, l = m_objectPoolTest.Length; i < l; ++i){
				if(Input.GetKeyDown(m_objectPoolTest[i].m_input)){
					if(m_spawnPool != null){
						SpawnObjectFromPool(m_objectPoolTest[i].m_objectToSpawn, m_spawnPool.position, m_spawnPool.rotation);
					}else{
						SpawnObjectFromPool(m_objectPoolTest[i].m_objectToSpawn, Vector3.zero, Quaternion.identity);
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

		if(m_enemyPoolDictionary[enemyType].Count == 0){
			Debug.LogError(enemyType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_enemyPoolDictionary[enemyType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.EnemyType);
		poolTracker.EnemyType = enemyType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnEnemyToPool(EnemyType enemyType, GameObject objectToReturn){
		objectToReturn.SetActive(false);
		m_enemyPoolDictionary[enemyType].Enqueue(objectToReturn);
	}


	public GameObject SpawnSpellFromPool(SpellType spellType, Vector3 position, Quaternion rotation){

		if(!m_spellPoolDictionary.ContainsKey(spellType)){
			Debug.LogError("Pool of " + spellType + " dosen't exist.");
			return null;
		}

		if(m_spellPoolDictionary[spellType].Count == 0){
			Debug.LogError(spellType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_spellPoolDictionary[spellType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.SpellType);
		poolTracker.SpellType = spellType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnSpellToPool(SpellType objectType, GameObject objectToReturn){
		PoolTracker poolTracker = objectToReturn.GetComponent<PoolTracker>();
        if(poolTracker != null){
            Destroy(poolTracker);
        }
		objectToReturn.SetActive(false);
		m_spellPoolDictionary[objectType].Enqueue(objectToReturn);
	}


	public GameObject SpawnObjectFromPool(ObjectType objectType, Vector3 position, Quaternion rotation){

		if(!m_objectPoolDictionary.ContainsKey(objectType)){
			Debug.LogError("Pool of " + objectType + " dosen't exist.");
			return null;
		}

		if(m_objectPoolDictionary[objectType].Count == 0){
			Debug.LogError(objectType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_objectPoolDictionary[objectType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.ObjectType);
		poolTracker.ObjectType = objectType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnObjectToPool(ObjectType objectType, GameObject objectToReturn){
		objectToReturn.SetActive(false);
		m_objectPoolDictionary[objectType].Enqueue(objectToReturn);
	}

	PoolTracker AddPoolTrackerComponent(GameObject objectToSpawn, PoolType poolType){
		PoolTracker poolTracker = objectToSpawn.AddComponent<PoolTracker>().GetComponent<PoolTracker>();
		poolTracker.PoolType = poolType;
		return poolTracker;
	}

	public void On_ReturnLifePotionInPool(){
		for (int i = 0, l = m_trackedObject.Count; i < l; ++i) {

			PoolTracker poolTracker = m_trackedObject.Dequeue();
			if(poolTracker == null){
				return;
			}
			
			Potion_Being_Used healPotion = poolTracker.GetComponent<Potion_Being_Used>();
			if(healPotion != null){
				poolTracker.ResetTrackedObject();
			}else{
				m_trackedObject.Enqueue(poolTracker);
			}
		}
	}

	public void On_ReturnAllInPool(){
		for (int i = 0, l = m_trackedObject.Count; i < l; ++i) {
			PoolTracker poolTracker = m_trackedObject.Dequeue();
			if(poolTracker != null){
				poolTracker.ResetTrackedObject();
			}
		}
	}

}
