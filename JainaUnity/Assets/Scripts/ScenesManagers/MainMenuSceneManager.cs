using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour {
    
    SaveManager m_saveManager;
    
    void Awake(){
        m_saveManager = SaveManager.Instance;
        if(m_saveManager != null){
            Destroy(m_saveManager.gameObject);
        }
    }

}
