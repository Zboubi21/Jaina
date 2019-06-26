using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour {

    public Vector3 GreenboxPosition;
    public Vector3 GreenboxScale;
    public bool drawGreenGizmos = true;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 BoxPosition = transform.TransformPoint(new Vector3(0, 0.5f, 1));
        Gizmos.DrawWireCube(BoxPosition, Vector3.one);

        if (drawGreenGizmos)
        {
            Gizmos.color = Color.green;
            Vector3 greenBoxPosition = transform.TransformPoint(new Vector3(GreenboxPosition.x, GreenboxPosition.y, GreenboxPosition.z));
            Gizmos.DrawWireCube(greenBoxPosition, GreenboxScale);
        }
    }


}
