using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReferenceScript : MonoBehaviour {

    public Image[] marksArray;
    public TextMeshProUGUI count;

    private void Awake()
    {
        //marksArray = GetComponentsInChildren<Image>();
    }
}
