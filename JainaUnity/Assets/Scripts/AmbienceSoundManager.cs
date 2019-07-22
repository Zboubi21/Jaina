using UnityEngine;

public class AmbienceSoundManager : MonoBehaviour {
    
    #region Singleton

	public static AmbienceSoundManager Instance;

	void Awake(){
		if(Instance == null){
			Instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of AmbienceSoundManager");
			gameObject.SetActive(false);
            Destroy(gameObject);
		}
	}

#endregion Singleton

}
