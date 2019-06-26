using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushController : MonoBehaviour {

	AmbushTrigger[] m_ambushTriggers;
	AmbushPoint[] m_ambushPoints;
	bool m_ambushIsStarded = false;

	void Awake(){
		m_ambushTriggers = GetComponentsInChildren<AmbushTrigger>();
		for(int i = 0, l = m_ambushTriggers.Length; i < l; ++i){
			m_ambushTriggers[i].AmbushController = this;
		}

		m_ambushPoints = GetComponentsInChildren<AmbushPoint>();
	}

	public void On_TriggerIsTriggered(){
		if(!m_ambushIsStarded){
			m_ambushIsStarded = true;
			for(int i = 0, l = m_ambushPoints.Length; i < l; ++i){
				m_ambushPoints[i].StartAmbush();
			}
		}
	}

}
