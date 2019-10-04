using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamerasManager : MonoBehaviour {

#region Singleton
	public static VideoCamerasManager Instance;
	void Awake(){
		if(Instance == null){
			Instance = this;
		}else{
			Debug.LogError("Two instance of VideoCamerasManager");
		}
	}
#endregion

	[SerializeField] bool m_useVideoCamerasManager = true;
	[Space]
	[SerializeField] GameObject m_pathsCanvas;
	[Space]
	[SerializeField] bool m_showCursor = true;
	PlayerManager m_playerManager;

	bool m_pathsCanvasIsActive = false;
	bool m_cursorIsShowed;

	List<FollowPathCreator> m_followPathCreators = new List<FollowPathCreator>();

	void Start(){
		m_playerManager = PlayerManager.Instance;
		m_pathsCanvas.SetActive(m_pathsCanvasIsActive);

		Cursor.visible = m_showCursor;
	}

	void Update(){
		if(!m_useVideoCamerasManager){
			return;
		}
		if(Input.GetKeyDown(KeyCode.V)){
			SetPathsCanvasIsActive(!m_pathsCanvasIsActive);
		}
	}

	public void SetFollowPathCreator(FollowPathCreator newFollowPathCreator){
		m_followPathCreators.Add(newFollowPathCreator);
	}

	public void DisableOtherFollowPath(){
		for (int i = 0, l = m_followPathCreators.Count; i < l; ++i){
			m_followPathCreators[i].ResetPath();
		}
	}

	public void SetPathsCanvasIsActive(bool isActive){
		m_pathsCanvasIsActive = isActive;
		m_pathsCanvas.SetActive(m_pathsCanvasIsActive);

		if(isActive){
			Cursor.visible = true;
		}else{
			Cursor.visible = m_showCursor;
		}

		// m_playerManager.m_videoCamerasManagerIsActive = m_pathsCanvasIsActive;
		// m_pauseGame.m_videoCamerasManagerIsActive = m_pathsCanvasIsActive;
	}

	public void StopUseVideoCameras(){
		for (int i = 0, l = m_followPathCreators.Count; i < l; ++i){
			m_followPathCreators[i].ResetPath();
		}
		SetPathsCanvasIsActive(false);
	}

}
