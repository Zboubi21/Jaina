using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JainaUI : UI, IPointerEnterHandler, IPointerExitHandler {
    
    [SerializeField] GameObject m_spellDescription;
    [SerializeField] GameObject m_markDescription;
    [SerializeField] GameObject m_selectedSpell;

    public override void OnPointerEnter(PointerEventData eventData) {
        // Debug.Log("Mouse enter");
        base.OnPointerEnter(eventData);

        UiController.On_UiPointerOver(this);
        m_selectedSpell.SetActive(MouseInUI);
        m_spellDescription.SetActive(MouseInUI);
        if(m_markDescription != null)
            m_markDescription.SetActive(false);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        // Debug.Log("Mouse exit");
        base.OnPointerExit(eventData);
        // CloseUI();
    }

    public void CloseUI(){
        m_selectedSpell.SetActive(false);
        m_spellDescription.SetActive(false);
        if(m_markDescription != null)
            m_markDescription.SetActive(false);
    }

}

// Before just use PointerOver
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JainaUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    
    [SerializeField] GameObject m_spellDescription;
    [SerializeField] GameObject m_markDescription;
    [SerializeField] GameObject m_selectedSpell;

    bool m_mouseInUI = false;
    public bool MouseInUI {
        get{
            return m_mouseInUI;
        }
        set{
            m_mouseInUI = value;
        }
    }

    bool m_isSelected = false;
    public bool IsSelected {
        get{
            return m_isSelected;
        }
        set{
            m_isSelected = value;
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

    public void OnPointerEnter(PointerEventData eventData) {
        // Debug.Log("Mouse enter");
        m_mouseInUI = true;

        if(m_uiController.SelectedUI != null){
            return;
        }
        m_uiController.On_UiPointerOver(this);
        m_selectedSpell.SetActive(m_mouseInUI);
        m_spellDescription.SetActive(m_mouseInUI);
        if(m_markDescription != null)
            m_markDescription.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        // Debug.Log("I click");
        m_isSelected =! m_isSelected;
        m_uiController.On_UiPointerClick(this, m_isSelected);
        m_selectedSpell.SetActive(m_isSelected);
        m_spellDescription.SetActive(m_isSelected);
        if(m_markDescription != null)
            m_markDescription.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Debug.Log("Mouse exit");
        m_mouseInUI = false;
        if(!m_isSelected){
            CloseUI();
        }
    }

    public void CloseUI(){
        m_isSelected = false;
        m_selectedSpell.SetActive(m_isSelected);
        m_spellDescription.SetActive(m_isSelected);
        if(m_markDescription != null)
            m_markDescription.SetActive(m_isSelected);
    }

}*/
