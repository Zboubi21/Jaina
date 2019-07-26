using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


public class Menu_UI : UI {

    Button ordalie;
    public GameObject warning;
    public UnityEvent OnMouseHover;
    public UnityEvent OnMouseExit;


    private void Start()
    {
        ordalie = GetComponent<Button>();
    }


    public override void OnPointerEnter(PointerEventData eventData) {

        if (!ordalie.interactable)
        {
            warning.SetActive(true);
            OnMouseHover.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {

        if (!ordalie.interactable)
        {
            warning.SetActive(false);
            OnMouseExit.Invoke();
        }
    }

}
