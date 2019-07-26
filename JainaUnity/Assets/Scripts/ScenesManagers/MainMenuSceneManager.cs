using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour {
    
    SaveManager m_saveManager;
    HistorySoundManager m_historySoundManager;
    ArcadeSoundManager m_arcadeSoundManager;
    
    void Awake(){
        m_saveManager = SaveManager.Instance;
        if(m_saveManager != null){
            Destroy(m_saveManager.gameObject);
        }

        m_historySoundManager = HistorySoundManager.Instance;
        if(m_historySoundManager != null){
            Destroy(m_historySoundManager.gameObject);
        }

        m_arcadeSoundManager = ArcadeSoundManager.Instance;
        if(m_arcadeSoundManager != null){
            Destroy(m_arcadeSoundManager.gameObject);
        }
    }

}
