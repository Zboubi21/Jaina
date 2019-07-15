using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetSoundSliderValue : MonoBehaviour {

	[Header("Sliders")]
	[SerializeField] Slider m_ambienceSlider;
	[SerializeField] Slider m_fxSlider;

	[Header("Audio mixers")]
	[SerializeField] AudioMixer m_musicAudioMixer;
	[SerializeField] AudioMixer m_fxAudioMixer;

	void OnEnable(){
		if(m_ambienceSlider != null && m_fxSlider != null){
			SetMusicSlider();
			SetFxSlider();
		}
	}
	void SetMusicSlider(){
		float value;
		m_musicAudioMixer.GetFloat("musicVolume", out value);
		m_ambienceSlider.value = value;
	}
	void SetFxSlider(){
		float value;
		m_musicAudioMixer.GetFloat("fxVolume", out value);
		m_fxSlider.value = value;
	}
	
}
