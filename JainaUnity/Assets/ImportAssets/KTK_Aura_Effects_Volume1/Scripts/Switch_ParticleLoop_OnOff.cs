//======================================
/*
@autor ktk.kumamoto
@date 2015.3.23 create
@note Switch_ParticleLoop_OnOff
*/
//======================================


using UnityEngine;
using System.Collections;

public class Switch_ParticleLoop_OnOff : MonoBehaviour {
	
	public bool LoopControler = true;
	
	void Awake(){
		loopEffectControl(LoopControler);
	}
	
	private void loopEffectControl(bool setLoop)
	{
		foreach (Transform child in transform)
		{
			ParticleSystem[] systems;
			systems = child.GetComponentsInChildren<ParticleSystem>(true);
			
			foreach(ParticleSystem ps in systems)
			{
				ps.loop = setLoop;
			}
		}
	}
}