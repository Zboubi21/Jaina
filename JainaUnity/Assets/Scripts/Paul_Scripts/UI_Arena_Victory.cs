﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Arena_Victory : MonoBehaviour
{
    public Waves_Methods arena;
    public GameObject clock;


    private void OnEnable()
    {
        float second = arena.Second;
        float minutes = arena.Minutes;
        TextMeshProUGUI textPro = clock.GetComponent<TextMeshProUGUI>();
        if (second < 10)
        {
            if (minutes < 10)
            {
                textPro.text = string.Format("0{0}:0{1}", (int)minutes, (int)second);
            }
            else
            {
                textPro.text = string.Format("{0}:0{1}", (int)minutes, (int)second);
            }

        }
        else
        {
            if (minutes < 10)
            {
                textPro.text = string.Format("0{0}:{1}", (int)minutes, (int)second);
            }
            else
            {
                textPro.text = string.Format("{0}:{1}", (int)minutes, (int)second);
            }
        }
    }
}