using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceScript : MonoBehaviour {

    public Image[] marksArray;

    private void Awake()
    {
        marksArray = GetComponentsInChildren<Image>();
    }
}
