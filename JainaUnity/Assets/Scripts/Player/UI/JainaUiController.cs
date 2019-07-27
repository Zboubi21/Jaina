using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JainaUiController : MonoBehaviour {
    
    [SerializeField] JainaUI[] m_uiImage;
    [SerializeField] UI[] m_ui;

    bool m_inUi = false;
    public bool InUi{
        get{
            return m_inUi;
        }
        set{
            m_inUi = value;
        }
    }

    JainaUI m_jainUiOver = null;
    public JainaUI JainUiOver{
        get{
            return m_jainUiOver;
        }
        set{
            m_jainUiOver = value;
        }
    }

    PlayerManager m_playerManager;
    bool m_playerInAutoAttack;
    CursorManagaer m_cursorManager;

    void Awake(){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            m_uiImage[i].UiController = this;
        }
        for (int i = 0, l = m_ui.Length; i < l; ++i) {
            m_ui[i].UiController = this;
        }
    }

    void Start(){
        m_playerManager = GetComponent<PlayerManager>();
        m_cursorManager = CursorManagaer.Instance;
    }

    public void On_UiPointerOver(JainaUI clickedUI){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
        }
    }

    public void On_UiPointerExit(){
        StartCoroutine(WaitToExit());
    }

    IEnumerator WaitToExit(){
        yield return new WaitForEndOfFrame();
        if(!IsOverUi()){
            for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
                m_uiImage[i].CloseUI();
            }
            m_inUi = false;
            m_playerManager.CanAutoAttackBecauseUi = true;

            m_cursorManager.ChangeCursorModeInMenu(false);
        }
    }
    
    public void CheckPlayerMode(){
        m_playerInAutoAttack = m_playerManager.InAutoAttack;
        if(!m_playerInAutoAttack){
            m_playerManager.CanAutoAttackBecauseUi = false;
            m_cursorManager.ChangeCursorModeInMenu(true);
        }
    }

    public bool CanShowSpell(){
        return !m_playerInAutoAttack;
    }

    public void On_PlayerLeftMouseUpClick(){
        if(IsOverUi()){
            m_playerManager.InAutoAttack = false;
            m_playerInAutoAttack = false;
            if(m_jainUiOver != null){
                m_jainUiOver.ShowUi();
            }
            m_playerManager.CanAutoAttackBecauseUi = false;
            m_cursorManager.ChangeCursorModeInMenu(true);
        }
    }

    bool IsOverUi(){
        bool isIn = false;
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(m_uiImage[i].MouseInUI){
                isIn = true;
            }
        }
        for (int i = 0, l = m_ui.Length; i < l; ++i) {
            if(m_ui[i].MouseInUI){
                isIn = true;
            }
        }
        return isIn;
    }

}

// Before just use PointerOver
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JainaUiController : MonoBehaviour {
    
    [SerializeField] JainaUI[] m_uiImage;

    JainaUI m_selectedUI;
    public JainaUI SelectedUI{
        get{
            return m_selectedUI;
        }
        set{
            m_selectedUI = value;
        }
    }

    void Awake(){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            m_uiImage[i].UiController = this;
        }
    }

    public void On_UiPointerOver(JainaUI clickedUI){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
        }
    }

    public void On_UiPointerClick(JainaUI clickedUI, bool isSelected){
        for (int i = 0, l = m_uiImage.Length; i < l; ++i) {
            if(clickedUI != m_uiImage[i]){
                m_uiImage[i].CloseUI();
            }
            m_selectedUI = isSelected ? clickedUI : null;
        }
    }
    
}*/
