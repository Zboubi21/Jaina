using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerMethods : MonoBehaviour
{
    Text timer;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timer.text = string.Format("{0}", (int)time);
    }
}
