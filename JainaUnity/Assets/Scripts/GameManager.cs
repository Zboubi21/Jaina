using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

#region Singleton
	public static GameManager Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of GameManager");
		}
	}
#endregion Singleton

    [SerializeField] float m_waitTimeTp = 0.25f;
    [SerializeField] GameObject m_mainMenuCanvas;
	[SerializeField] Animator m_blackScreenAnimator;

    Camera m_mainMenuCamera;

    void Start(){
        m_mainMenuCamera = GetComponentInChildren<Camera>();
    }
	public void StartAtPosition(Transform newPos){
        StartCoroutine(WaitTimeToQuit(newPos));
        // StartCoroutine(WaitTimeToMainMenu());
	}
    IEnumerator WaitTimeToQuit(Transform newPos){
        m_blackScreenAnimator.SetTrigger("BlackScreen");
		yield return new WaitForSeconds(m_waitTimeTp);
        m_mainMenuCanvas.SetActive(false);
		PlayerManager.Instance.SetPlayerMenuMode(false, newPos);
		CameraManager.Instance.ResetPosition();
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
    
}
