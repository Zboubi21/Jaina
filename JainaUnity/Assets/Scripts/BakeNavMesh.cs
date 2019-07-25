using UnityEngine;

public class BakeNavMesh : MonoBehaviour {

#region Singleton
	public static BakeNavMesh Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            // DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of BakeNavMesh");
            // gameObject.SetActive(false);
            // Destroy(gameObject);
		}
	}
#endregion Singleton

    

}
