using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

#region Singleton
	public static GameManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            // DontDestroyOnLoad(gameObject);
		}else{
			Debug.LogError("Two instance of GameManager");
            // gameObject.SetActive(false);
            // Destroy(gameObject);
		}
	}
#endregion Singleton

    [SerializeField] float m_waitTimeTp = 0.25f;
    [SerializeField] GameObject m_mainMenuCanvas;
	[SerializeField] Animator m_blackScreenAnimator;

    [Header("Game mode")]
    [SerializeField] Transform m_mainMenuPosition;
    [SerializeField] Transform m_levelDesignPosition;
    [SerializeField] Transform m_arenaPosition;

    [Header("Player settings")]
    public PlayerSettings m_playerSettings = new PlayerSettings();
	[System.Serializable] public class PlayerSettings {
        public bool m_useCanGoInArcadeModeDebuger = false;
        public bool m_canGoInArcadeMode = false;
        public Button m_arcadeModeBtn;
	}

    Camera m_mainMenuCamera;
    MainMenu m_mainMenu;

    int m_canArcadeModeNb = 0; // 0 = FALSE, 1 = TRUE
    string m_canArcade = "CanArcade";

    // GameObject[] m_levelDesignObjects;
    // string m_levelDesignObjectsName = "LevelDesignObjects";
    // GameObject[] m_arenaObjects;
    // string m_arenaObjectsName = "ArenaObjects";
    // GameObject[] m_mainMenuObjects;
    // string m_mainMenuObjectsName = "MainMenuObjects";

    void Start(){
        m_mainMenuCamera = GetComponentInChildren<Camera>();

        m_mainMenu = GetComponent<MainMenu>();

        m_canArcadeModeNb = PlayerPrefs.GetInt(m_canArcade);

        if(m_playerSettings.m_useCanGoInArcadeModeDebuger){
            SetArcadeModeBtn(m_playerSettings.m_canGoInArcadeMode);
        }else{
            if(m_canArcadeModeNb == 0){
                SetArcadeModeBtn(false);
            }else if(m_canArcadeModeNb == 1){
                SetArcadeModeBtn(true);
            }
        }
    }

    void Update(){
        // if(Input.GetKeyDown(KeyCode.G)){
        //     SetArcadeModeBtn(true);
        // }
        // if(Input.GetKeyDown(KeyCode.R)){
        //     PlayerPrefs.DeleteKey(m_canArcade);
        //     SetArcadeModeBtn(false);
        // }

        // if(Input.GetKeyDown(KeyCode.K)){
        //     FindAndSetObjects(m_levelDesignObjects, m_levelDesignObjectsName, true);
        // }
    }

	// public void StartStory(){
    //     FindAndSetObjects(m_levelDesignObjects, m_levelDesignObjectsName, true);
    //     FindAndSetObjects(m_arenaObjects, m_arenaObjectsName, false);
    //     StartCoroutine(WaitTimeToStartGame(m_levelDesignPosition));
	// }
    // public void StartArena(){
    //     FindAndSetObjects(m_levelDesignObjects, m_levelDesignObjectsName, false);
    //     FindAndSetObjects(m_arenaObjects, m_arenaObjectsName, true);
    //     StartCoroutine(WaitTimeToStartGame(m_arenaPosition));
	// }
    // void FindAndSetObjects(GameObject[] objects, string tagName, bool setActive){

    //     // if(tagName == "LevelDesignObjects"){
    //     //     if(m_levelDesignObjects == null){
    //     //         objects = GameObject.FindGameObjectsWithTag(tagName);
    //     //     }
    //     // }else if(tagName == "ArenaObjects"){
    //     //     if(m_arenaObjects == null){
    //     //         objects = GameObject.FindGameObjectsWithTag(tagName);
    //     //     }
    //     // }else if(tagName == "MainMenuObjects"){
    //     //     if(m_mainMenuObjects == null){
    //     //         objects = GameObject.FindGameObjectsWithTag(tagName);
    //     //     }
    //     // }
    //     objects = GameObject.FindGameObjectsWithTag(tagName);
    //     for (int i = 0, l = objects.Length; i < l; ++i){
    //         objects[i].SetActive(setActive);
    //     }

    //     if(tagName == "LevelDesignObjects"){
    //         m_levelDesignObjects = objects;
    //     }else if(tagName == "ArenaObjects"){
    //         m_arenaObjects = objects;
    //     }else if(tagName == "MainMenuObjects"){
    //         m_mainMenuObjects = objects;
    //     }
    // }
    IEnumerator WaitTimeToStartGame(Transform newPos){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        m_mainMenuCanvas.SetActive(false);
		PlayerManager.Instance.SetPlayerMenuMode(false, newPos);
		CameraManager.Instance.ResetPosition();
        CameraManager.Instance.CanMoveCamera = true;
		m_mainMenuCamera.enabled = false;
        // FindAndSetObjects(m_mainMenuObjects, m_mainMenuObjectsName, false);
	}

    public void ReturnToMainMenu(){
        // FindAndSetObjects(m_mainMenuObjects, m_mainMenuObjectsName, true);
        StartCoroutine(WaitTimeToMainMenu(m_mainMenuPosition));
    }
    IEnumerator WaitTimeToMainMenu(Transform newPos){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        // SaveManager.Instance.ReloadScene();
        m_mainMenu.StartLevel(0);
        // yield return new WaitForFixedUpdate();
        // m_mainMenuCanvas.SetActive(true);
		// PlayerManager.Instance.SetPlayerMenuMode(true, newPos);
		// m_mainMenuCamera.enabled = true;
        // FindAndSetObjects(m_levelDesignObjects, m_levelDesignObjectsName, false);
        // FindAndSetObjects(m_arenaObjects, m_arenaObjectsName, false);
	}

    public void SetArcadeModeBtn(bool inArcadeMode){
        m_playerSettings.m_arcadeModeBtn.interactable = inArcadeMode;

        if(inArcadeMode){
            PlayerPrefs.SetInt(m_canArcade, 1); // 1 = TRUE
            m_canArcadeModeNb = 1;
        }else{
            PlayerPrefs.SetInt(m_canArcade, 0); // 0 = FALSE
            m_canArcadeModeNb = 0;
        }
    }

    public void StartLevel(int sceneNbr){
        StartCoroutine(WaitTimeToLoadScene(sceneNbr));
    }
    IEnumerator WaitTimeToLoadScene(int sceneNbr){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        // m_mainMenuCanvas.SetActive(false);  
        m_mainMenu.StartLevel(sceneNbr);
        // m_mainMenuCanvas.SetActive(false);
		// PlayerManager.Instance.SetPlayerMenuMode(false, newPos);
		// CameraManager.Instance.ResetPosition();
        // CameraManager.Instance.CanMoveCamera = true;
		// m_mainMenuCamera.enabled = false;
        // FindAndSetObjects(m_mainMenuObjects, m_mainMenuObjectsName, false);
	}
    
}
