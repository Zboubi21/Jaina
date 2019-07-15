using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

[SerializeField] float m_waitTimeToQuit = 0.175f;

void Awake(){
	SetResolution();
}

// ---------------------------------------------- Quitter ----------------------------------------------
	public void Quit(){											// Fonction pour quitter le jeu
		StartCoroutine(WaitTimeToQuit());
	}

	IEnumerator WaitTimeToQuit(){
		yield return new WaitForSeconds(m_waitTimeToQuit);
#if UNITY_EDITOR												// Si on est sur Unity Editor alors :
		if(Application.isPlaying){								// Si on est en mode "play" dans l'éditeur d'Unity
			UnityEditor.EditorApplication.isPlaying = false;	// Stop le mode play
		}
#else //UNITY_EDITOR											// Sinon :
		Application.Quit();										// Permet de quitter l'application
#endif //UNITY_EDITOR											// Fini
	}

// ---------------------------------------------- StartLevel ----------------------------------------------
	public void StartLevel(int numLevel){							// Fonction pour commencer le jeu
#if UNITY_5_4_OR_NEWER												// Si on est sur une version de Unity >= à la 5.4 alors :
	SceneManager.LoadScene(numLevel);	// Lance le numéro du level
#else //UNITY_5_4_OR_NEWER											// Si on est sur une version de Unity <= à la 5.4 alors :
	Application.LoadLevel(numLevel);								// Lance le numéro du level
#endif //UNITY_5_4_OR_NEWER											// Fini
	}

	public void RestartLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

// ---------------------------------------------- Régler le son ----------------------------------------------
	[SerializeField] private AudioMixer m_musicAudioMixer;
	[SerializeField] private AudioMixer m_fxAudioMixer;
	public void SetVolumeMusic(float volume){
		m_musicAudioMixer.SetFloat("musicVolume", volume);
	}
	public void SetVolumeFX(float volume){
		m_fxAudioMixer.SetFloat("fxVolume", volume);
	}

// ---------------------------------------------- Régler les paramètres graphiques ----------------------------------------------
	public void SetQuality(int qualityIndex){
		QualitySettings.SetQualityLevel(qualityIndex);
	}

// ---------------------------------------------- Mettre/Enlever le mode pleine écran ----------------------------------------------
	public void SetFullScreen(bool isFullscreen){
		Screen.fullScreen = isFullscreen;
		//Debug.Log(isFullscreen);
	}

// ---------------------------------------------- Gestion de la résolution de l'écran ----------------------------------------------

	public void SetResolution(int resolutionIndex){
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	private Resolution[] resolutions;
	public Dropdown m_resolutionDropdown;
	void SetResolution(){
		if(m_resolutionDropdown != null){
			resolutions = Screen.resolutions;

			m_resolutionDropdown.ClearOptions();

			List<string> options = new List<string>();

			int currentResolutionIndex = 0;

			for(int i = 0; i < resolutions.Length; i++){
				string option = resolutions[i].width + "x" + resolutions[i].height;
				options.Add(option);

				if( (resolutions[i].width == Screen.currentResolution.width) && (resolutions[i].height == Screen.currentResolution.height) ){
					currentResolutionIndex = i;
				}
			}

			m_resolutionDropdown.AddOptions(options);
			m_resolutionDropdown.value = currentResolutionIndex;
			m_resolutionDropdown.RefreshShownValue();
		}
	}

}
