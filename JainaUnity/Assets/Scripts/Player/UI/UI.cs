﻿using System.Collections;
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
        m_mouseInUI = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        // Debug.Log("Mouse exit");
        m_mouseInUI = false;
        UiController.On_UiPointerExit();
    }

}