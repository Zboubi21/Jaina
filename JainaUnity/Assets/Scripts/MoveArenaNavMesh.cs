using UnityEngine;

public class MoveArenaNavMesh : MonoBehaviour {
    
    [SerializeField] Vector3 m_moveNavMeshPosValue;

    public void MoveNavMeshPosition(){
        transform.position = transform.position + m_moveNavMeshPosValue;
    }

}
