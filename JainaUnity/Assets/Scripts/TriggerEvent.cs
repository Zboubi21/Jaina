using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {

    [SerializeField] public UnityEvent m_triggerEvent;
    [SerializeField] public Color m_gizmosColor = Color.blue;

    BoxCollider m_boxColl;
    bool m_checkpointIsTake = false;

    public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player")){
            if(!m_checkpointIsTake){
                m_checkpointIsTake = true;
                m_triggerEvent.Invoke();
            }
        }
    }

    void OnDrawGizmos(){

        if(BoxColl == null){
            return;
        }

        Gizmos.color = m_gizmosColor;

        Vector3 boxPos;
        boxPos = transform.position;

        Vector3 boxSize;
        boxSize = new Vector3(BoxColl.size.x * transform.localScale.x, BoxColl.size.y * transform.localScale.y, BoxColl.size.z * transform.localScale.z);

        Vector3 boxCenter;
        boxCenter = new Vector3(BoxColl.center.x * transform.localScale.x, BoxColl.center.y * transform.localScale.y, BoxColl.center.z * transform.localScale.z);

        Quaternion boxRotation = transform.rotation;

        GL.PushMatrix();
        Matrix4x4 boxRotationMatrix = Matrix4x4.TRS(boxPos, boxRotation, Vector3.one);
        Gizmos.matrix = boxRotationMatrix;  
        Gizmos.DrawWireCube(boxCenter, boxSize);
        GL.PopMatrix();
	}

}
