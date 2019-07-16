﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;		// Permet d'accéder aux UI
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PauseGame : MainMenu {
	//[SerializeField] private EventSystem m_optionsEventSystem;

	public bool m_canPaused = true;

	[SerializeField] Transform m_pausedCanvas;
	[SerializeField] Transform m_optionsCanvas;

	[SerializeField] AudioMixerSnapshot m_pausedSnapshot;
	[SerializeField] AudioMixerSnapshot m_unpausedSnapshot;

	bool m_pause = false;
	bool m_pauseKey = false;

	void Start(){
		m_pause = false;
		
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

}