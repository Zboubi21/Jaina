using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour {

    [Header("Parameters")]
    [SerializeField] int m_checkpointNumber = 0;
    [SerializeField] Transform m_savePosition;
    [SerializeField] BoatCheckpoint m_boat;

    [Header("Gizmos")]
    [SerializeField] Color m_gizmosColor = Color.red;
    [SerializeField] float m_gizmosRadius = 0.25f;

    BoxCollider m_boxColl;
    bool m_checkpointIsTake = false;
    SaveManager m_saveManager;

    public BoxCollider BoxColl{
        get{
            return GetComponent<BoxCollider>();
        }
    }

    void Start(){
        m_saveManager = SaveManager.Instance;
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player")){
            if(!m_checkpointIsTake && m_saveManager.ActualCheckpointNumber <= m_checkpointNumber){
                m_checkpointIsTake = true;
                m_saveManager.On_CheckpointIsTake(m_savePosition, m_checkpointNumber);
                m_boat.On_CheckpointIsTake();
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

        // Quaternion boxRotation = transform.localRotation;
        Quaternion boxRotation = transform.rotation;

        if(m_savePosition != null){
            Gizmos.DrawSphere(m_savePosition.position, m_gizmosRadius);
            Gizmos.DrawLine(transform.position, m_savePosition.position);
        }

        GL.PushMatrix();
        Matrix4x4 boxRotationMatrix = Matrix4x4.TRS(boxPos, boxRotation, Vector3.one);
        Gizmos.matrix = boxRotationMatrix;  
        Gizmos.DrawWireCube(boxCenter, boxSize);
        GL.PopMatrix();


	}

}
