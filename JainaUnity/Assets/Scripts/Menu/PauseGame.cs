using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseGame : MainMenu {
	
	[Space]
	//[SerializeField] private EventSystem m_optionsEventSystem;

	[SerializeField] Transform m_pausedCanvas;
	[SerializeField] Transform m_optionsCanvas;

	[SerializeField] AudioMixerSnapshot m_pausedSnapshot;
	[SerializeField] AudioMixerSnapshot m_unpausedSnapshot;

	bool m_canPaused = true;
	public bool CanPaused{
        get{
            return m_canPaused;
        }
        set{
            m_canPaused = value;
        }
    }

	bool m_pause = false;
	bool m_pauseKey = false;
	PlayerManager m_playerManager;

    

    void OnEnable(){
		m_canPaused = true;
	}

	void Start(){
		m_pause = false;
		m_playerManager = GetComponent<PlayerManager>();
		
		if(m_pausedCanvas != null){
			m_pausedCanvas.gameObject.SetActive(false);	
		}
		if(m_optionsCanvas != null){
			m_optionsCanvas.gameObject.SetActive(false);
		}
		Time.timeScale = 1;
	}
	
	void Update(){
		m_pauseKey = Input.GetButtonDown("Pause");
		if(m_canPaused && m_pauseKey){
			Resume();
		}
	}

	public void Resume(){
		
		m_pause = !m_pause;

		m_playerManager.InPauseGame = m_pause;

		if(m_pause){
			Time.timeScale = 0;
			m_pausedCanvas.gameObject.SetActive(true);
			if(m_unpausedSnapshot != null){
				m_pausedSnapshot.TransitionTo(0.01f);
			}
		}else{
			Time.timeScale = 1;
			m_pausedCanvas.gameObject.SetActive(false);
			
			if(m_optionsCanvas != null){
				m_optionsCanvas.gameObject.SetActive(false);
			}

			if(m_unpausedSnapshot != null){
				m_unpausedSnapshot.TransitionTo(0.01f);
			}
		}
	}

	public void RestartLevel(){
		SaveManager m_saveManager = SaveManager.Instance;
		m_saveManager.StartCoroutine(m_saveManager.On_RestartFromLastCheckPoint());
	}

	public void RestartArena(){
		ObjectPooler.Instance.On_ReturnAllInPool();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void RestartGame(){
		SaveManager m_saveManager = SaveManager.Instance;
		m_saveManager.StartCoroutine(m_saveManager.On_RestartGame());
	}

	public void ReturnToMainMenu(){
		m_canPaused = false;
		Resume();
		StartLevel(0); //
		// GameManager.Instance.ReturnToMainMenu();
        ObjectPooler.Instance.On_ReturnAllInPool();
	}

	public void ReturnToMainMenuWithArcadeModeAnimator(){
		m_playerManager.On_AnimateArcadeModeAnimator();
		m_canPaused = false;
		StartLevel(0); //
		// GameManager.Instance.ReturnToMainMenu();
        ObjectPooler.Instance.On_ReturnAllInPool();
	}

	public override void Quit(){
		m_canPaused = false;
		Resume();
		base.Quit();
	}

}
