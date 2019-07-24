using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

#region Singleton
	public static GameManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of GameManager");
            gameObject.SetActive(false);
            Destroy(gameObject);
		}
	}
#endregion Singleton

    [SerializeField] float m_waitTimeTp = 0.25f;
    [SerializeField] GameObject m_mainMenuCanvas;
	[SerializeField] Animator m_blackScreenAnimator;

    [Header("Game mode")]
    [SerializeField] GameObject m_levelDesignObjects;
    [SerializeField] Transform m_levelDesignPosition;
    [SerializeField] GameObject m_arenaObjects;
    [SerializeField] Transform m_arenaPosition;

    [Header("Player settings")]
    public PlayerSettings m_playerSettings = new PlayerSettings();
	[System.Serializable] public class PlayerSettings {
		public bool m_playerCanDie = true;
		public bool m_useSymetricalHudSpellAnim = true;
		public PlayerState m_playerStartState;
		public bool m_startInMenuMode = true;
        [Space]
        public bool m_useCanGoInArcadeModeDebuger = false;
        public bool m_canGoInArcadeMode = false;
        public Button m_arcadeModeBtn;
	}

    Camera m_mainMenuCamera;

    int m_canArcadeModeNb = 0; // 0 = FALSE, 1 = TRUE
    string m_canArcade = "CanArcade";

    void Start(){
        m_mainMenuCamera = GetComponentInChildren<Camera>();

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
    }

	public void StartStory(){
        if(m_levelDesignObjects != null && m_arenaObjects != null){
            m_levelDesignObjects.SetActive(true);
            m_arenaObjects.SetActive(false);
        }
        StartCoroutine(WaitTimeToStartGame(m_levelDesignPosition));
	}
    public void StartArena(){
        if(m_levelDesignObjects != null && m_arenaObjects != null){
            m_levelDesignObjects.SetActive(false);
            m_arenaObjects.SetActive(true);
        }
        StartCoroutine(WaitTimeToStartGame(m_arenaPosition));
	}
    IEnumerator WaitTimeToStartGame(Transform newPos){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        m_mainMenuCanvas.SetActive(false);
		PlayerManager.Instance.SetPlayerMenuMode(false, newPos);
		CameraManager.Instance.ResetPosition();
        CameraManager.Instance.CanMoveCamera = true;
		m_mainMenuCamera.enabled = false;
	}

    public void ReturnToMainMenu(){
        StartCoroutine(WaitTimeToMainMenu());
    }
    IEnumerator WaitTimeToMainMenu(){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        m_mainMenuCanvas.SetActive(true);
		PlayerManager.Instance.SetPlayerMenuMode(true);
		m_mainMenuCamera.enabled = true;
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
    
}
