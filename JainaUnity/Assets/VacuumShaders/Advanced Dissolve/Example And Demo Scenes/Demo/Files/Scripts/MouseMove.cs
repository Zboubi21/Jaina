using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

namespace AdvancedDissolve_Example
{
    public class MouseMove : MonoBehaviour
    { 
        // Update is called once per frame 
        void Update()
        {
            transform.position -= transform.up * Input.mouseScrollDelta.y;
        }
    } 
} 