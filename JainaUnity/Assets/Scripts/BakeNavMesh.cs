using UnityEngine;
using UnityEngine.AI;

public class BakeNavMesh : MonoBehaviour {

#region Singleton
	// public static BakeNavMesh Instance;
    // void Awake(){
	// 	if(Instance == null){
	// 		Instance = this;
    //         // DontDestroyOnLoad(gameObject);
	// 	}else{
	// 		Debug.LogError("Two instance of BakeNavMesh");
    //         // gameObject.SetActive(false);
    //         // Destroy(gameObject);
	// 	}
	// }
#endregion Singleton

    NavMeshSurface[] m_navMeshes;
    public NavMeshSurface[] NavMeshes{
        get{
            return m_navMeshes;
        }
        set{
            m_navMeshes = value;
        }
    }

    // void Update(){
    //     if(Input.GetKeyDown(KeyCode.P)){
    //         BakeNavMeshInRealTime();
    //     }
    // }

    // public void BakeNavMeshInRealTime(){
    //     for(int i = 0, l = m_navMeshes.Length; i < l; ++i){
    //         // m_navMeshes[i].BuildNavMesh();
    //         // m_navMeshes[i].UpdateNavMesh(m_navMeshes[i].navMeshData);
    //     }
    // }

}
