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

	[System.Serializable]
	public class Pool{
		public string m_name;
		public GameObject m_prefab;
		public int m_size;
	}

	[SerializeField] List<Pool> m_pools;
	Dictionary<string, Queue<GameObject>> m_poolDictionary;

	void Start(){
		m_poolDictionary = new Dictionary<string, Queue<GameObject>>();

		foreach(Pool pool in m_pools){
			Queue<GameObject> objectPool = new Queue<GameObject>();

			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform);
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

		GameObject objectToSPawn = m_poolDictionary[name].Dequeue();

		objectToSPawn.transform.position = position;
		objectToSPawn.transform.rotation = rotation;
		objectToSPawn.SetActive(true);

		m_poolDictionary[name].Enqueue(objectToSPawn);

		return objectToSPawn;
	}

	public void ReturnToPool(string name, GameObject objectToReturn){
		
		objectToReturn.SetActive(false);
		m_poolDictionary[name].Enqueue(objectToReturn);

	}

	void Update(){
		// Debug.LogError("Dictionnary count = " + m_poolDictionary.Count);	
	}

}
