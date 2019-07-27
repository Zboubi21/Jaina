using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManagaer : MonoBehaviour {

#region Singleton
	public static CursorManagaer Instance;
    void Awake(){
		if(Instance == null){
			Instance = this;
            // DontDestroyOnLoad(gameObject);
		}else{
			Debug.LogError("Two instance of CursorManagaer");
            // gameObject.SetActive(false);
            // Destroy(gameObject);
		}
	}
#endregion Singleton

    public StartCursorState m_startCursorState;
    public enum StartCursorState {
		None,
		MenuCursor,
		FightCursor,
	}

    [SerializeField] Texture2D m_menuCursor;
    [SerializeField] Texture2D m_fightCursor;
    [SerializeField] Vector2 m_hotSpot = new Vector2(2048, 2048);

    void Start(){
        switch(m_startCursorState){
			case StartCursorState.None:
			break;
            case StartCursorState.MenuCursor:
                ChangeCursorModeInMenu(true);
			break;
            case StartCursorState.FightCursor:
                ChangeCursorModeInMenu(false);
			break;
        }
    }

    public void ChangeCursorModeInMenu(bool inMenu){
        if(inMenu){
            Cursor.SetCursor(m_menuCursor, m_hotSpot, CursorMode.Auto);
        }else{
            Cursor.SetCursor(m_fightCursor, m_hotSpot, CursorMode.Auto);
        }
    }
    
}
