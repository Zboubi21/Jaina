using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AmbushTrigger : MonoBehaviour {

	[Header("Gizmos")]
	[SerializeField] Color m_gizmosColor = Color.magenta;

	AmbushController m_ambushController;
    public AmbushController AmbushController
    {
        get
        {
            return m_ambushController;
        }

        set
        {
            m_ambushController = value;
        }
    }

    void OnDrawGizmosSelected(){
		Gizmos.color = m_gizmosColor;
		BoxCollider bc = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube(transform.position + bc.center, bc.size);
	}

	void OnTriggerEnter(Collider col){
		if(col.CompareTag("Player")){
			m_ambushController.On_TriggerIsTriggered();
		}
	}

}
