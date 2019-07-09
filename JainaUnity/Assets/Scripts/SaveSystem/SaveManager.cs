using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

#region Singleton
	public static SaveManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of SaveManager");
            Destroy(gameObject);
		}
	}
#endregion Singleton

    [SerializeField] GameObject m_checkPointCanvas;

    Vector3 m_savePosition;

    public void On_CheckpointIsTake(Transform savePosition){
        m_savePosition = savePosition.position;
    }

}
