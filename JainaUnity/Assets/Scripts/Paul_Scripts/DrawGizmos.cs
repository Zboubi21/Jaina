using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour {

    /*public Vector3 GreenboxPosition;
    public Vector3 GreenboxScale;*/
    public bool drawGreenGizmos = true;

    EnemyController control;


    private void OnDrawGizmosSelected()
    {
        control = GetComponent<EnemyController>();
        Gizmos.color = Color.red;
        Vector3 BoxPosition = transform.TransformPoint(new Vector3(control.RedBoxPosition.x, control.RedBoxPosition.y, control.RedBoxPosition.z));
        Gizmos.DrawWireCube(BoxPosition, new Vector3(control.RedBoxScale.x, control.RedBoxScale.y, control.RedBoxScale.z));

        if (drawGreenGizmos)
        {
            Gizmos.color = Color.green;
            Vector3 greenBoxPosition = transform.TransformPoint(new Vector3(control.GreenBoxPosition.x, control.GreenBoxPosition.y, control.GreenBoxPosition.z));
            Gizmos.DrawWireCube(greenBoxPosition, new Vector3(control.GreenBoxScale.x, control.GreenBoxScale.y, control.GreenBoxScale.z));
        }
    }


}
