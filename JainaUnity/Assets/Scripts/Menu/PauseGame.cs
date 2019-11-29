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
	[Space]
	[SerializeField] Animator m_arenaDieCanvas;

	bool m_canPaused;
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
	CursorManagaer m_curosrManager;
    
    void OnEnable(){
        StartCoroutine(WaitForFadeInToEscape());
	}
    
	void Start(){
		m_pause = false;
		m_playerManager = GetComponent<PlayerManager>();
		m_curosrManager = CursorManagaer.Instance;
		
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
        // Debug.Log(m_canPaused);
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

			m_curosrManager.ChangeCursorModeInMenu(true);

			if(m_unpausedSnapshot != null){
				m_pausedSnapshot.TransitionTo(0.01f);
			}
		}else{
			Time.timeScale = 1;
			m_pausedCanvas.gameObject.SetActive(false);

			m_curosrManager.ChangeCursorModeInMenu(false);
			
			if(m_optionsCanvas != null){
				m_optionsCanvas.gameObject.SetActive(false);
			}
			if(m_unpausedSnapshot != null){
				m_unpausedSnapshot.TransitionTo(0.01f);
			}
		}
	}

	public void RestartLevel(bool fromPauseMenu){
		// SaveManager m_saveManager = SaveManager.Instance;
		// m_saveManager.StartCoroutine(m_saveManager.On_RestartFromLastCheckPoint());
        m_canPaused = false;
		if(fromPauseMenu){
			Resume();
			m_blackScreenAnimator.SetTrigger("BlackScreen");
		}
		StartCoroutine(RestartLevelCorout());
	}
    IEnumerator WaitForFadeInToEscape()
    {
        yield return new WaitForSeconds(1f);
        m_canPaused = true;
        StopCoroutine(WaitForFadeInToEscape());
    }
    IEnumerator RestartLevelCorout(){
		yield return new WaitForSeconds(m_waitTimeToQuit);
		SaveManager m_saveManager = SaveManager.Instance;
		m_saveManager.StartCoroutine(m_saveManager.On_RestartFromLastCheckPoint());
	}
	public void RestartArena(bool fromPauseMenu){
		// ObjectPooler.Instance.On_ReturnAllInPool();
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		m_canPaused = false;
		if(fromPauseMenu){
			Resume();
		}
		StartCoroutine(RestartArenaCorout(fromPauseMenu));
	}
	IEnumerator RestartArenaCorout(bool fromPauseMenu){
		if(fromPauseMenu){
			m_blackScreenAnimator.SetTrigger("BlackScreen");
			yield return new WaitForSeconds(m_waitTimeToQuit);
		}else{
			yield return new WaitForSeconds(0.5f);
			m_arenaDieCanvas.SetTrigger("FadeIn");
			yield return new WaitForSeconds(2.25f);
		}
		ObjectPooler.Instance.On_ReturnAllInPool();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void RestartGame(){
		SaveManager m_saveManager = SaveManager.Instance;
		m_saveManager.StartCoroutine(m_saveManager.On_RestartGame());
	}

	public void ReturnToMainMenu(){
		// m_canPaused = false;
		// Resume();
		// StartLevel(0); //
		// // GameManager.Instance.ReturnToMainMenu();
        // ObjectPooler.Instance.On_ReturnAllInPool();
		m_canPaused = false;
		Resume();
		StartCoroutine(ReturnToMainMenuCorout());
	}
	IEnumerator ReturnToMainMenuCorout(){
		m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeToQuit);
		StartLevel(0);
        ObjectPooler.Instance.On_ReturnAllInPool();
	}
    public void ReturnToMainMenuFromArena(){
        m_canPaused = false;
        // StartLevel(0); //
        // ObjectPooler.Instance.On_ReturnAllInPool();
		StartCoroutine(ReturnToMainMenuCorout());
    }

    public void ReturnToMainMenuWithArcadeModeAnimator(){
		// m_playerManager.On_AnimateArcadeModeAnimator();
		// m_canPaused = false;
		// StartLevel(0); //
        // ObjectPooler.Instance.On_ReturnAllInPool();
		StartCoroutine(ReturnToMainMenuWithArcadeModeAnimatorCorout());
	}
	IEnumerator ReturnToMainMenuWithArcadeModeAnimatorCorout(){
		m_canPaused = false;
		m_blackScreenAnimator.SetTrigger("BlackScreen");
		m_playerManager.On_AnimateArcadeModeAnimator();
		yield return new WaitForSeconds(m_waitTimeToQuit);
		StartLevel(0); //
        ObjectPooler.Instance.On_ReturnAllInPool();
	}

	public override void Quit(){
		m_canPaused = false;
		Resume();
		base.Quit();
	}

}
