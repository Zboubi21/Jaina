using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField] List<Pool> m_pools;
	[System.Serializable] public class Pool{
        public string m_name;
        public GameObject m_prefab;
		public int m_size;
    }

	

	Dictionary<string, Queue<GameObject>> m_poolDictionary;

	void Start(){
		m_poolDictionary = new Dictionary<string, Queue<GameObject>>();

		foreach(Pool pool in m_pools){
			Queue<GameObject> objectPool = new Queue<GameObject>();

			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}

			m_poolDictionary.Add(pool.m_name, objectPool);
		}
	}

	public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation){

		if(!m_poolDictionary.ContainsKey(name)){
			Debug.LogWarning("Pool with name " + name + " dosen't exist.");
			return null;
		}

		if(m_poolDictionary[name].Peek().activeSelf){
			Debug.LogError("All " + name + " are already active!");
			return null;
		}

		GameObject objectToSpawn = m_poolDictionary[name].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		m_poolDictionary[name].Enqueue(objectToSpawn);

		return objectToSpawn;
	}

	public void ReturnToPool(string name, GameObject objectToReturn){
		
		objectToReturn.SetActive(false);
		m_poolDictionary[name].Enqueue(objectToReturn);

	}

	void Update(){
		// Debug.LogError("Dictionnary count = " + m_poolDictionary.Count);	
	}

}
