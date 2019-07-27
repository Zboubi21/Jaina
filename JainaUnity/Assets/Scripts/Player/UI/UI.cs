using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    bool m_mouseInUI = false;
    public bool MouseInUI {
        get{
            return m_mouseInUI;
        }
        set{
            m_mouseInUI = value;
        }
    }

    JainaUiController m_uiController;
    public JainaUiController UiController {
        get{
            return m_uiController;
        }
        set{
            m_uiController = value;
        }
    }

    void OnDisable(){
        m_mouseInUI = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        // Debug.Log("Mouse enter");
        if(m_uiController != null){
            if(!m_uiController.InUi){
                m_uiController.CheckPlayerMode();
            }
            m_uiController.InUi = true;
        }
        m_mouseInUI = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        // Debug.Log("Mouse exit");
        m_mouseInUI = false;
        if(m_uiController != null){
            m_uiController.On_UiPointerExit();
        }
    }

}
