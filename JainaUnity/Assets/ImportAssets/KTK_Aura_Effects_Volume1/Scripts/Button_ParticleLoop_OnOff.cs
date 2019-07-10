//======================================
/*
@autor ktk.kumamoto
@date 2015.3.23 create
@note Button_ParticleLoop_OnOff
*/
//======================================

using UnityEngine;
using System.Collections;

public class Button_ParticleLoop_OnOff : MonoBehaviour {
	
	private bool LoopControler = true;
	
	private bool isChecked = true;
	public GameObject Maneger;
	private string Effect_State = "Effect Loop:ON";
	
	void OnGUI()
	{
		Rect rect1 = new Rect(170, 80, 400, 30);
		isChecked = GUI.Toggle(rect1, isChecked, Effect_State);
		if (Maneger != null){
			if (isChecked ) {
				Maneger.SendMessage ("ParticleLoopStatus",LoopControler);
				Effect_State = "Effect Loop:ON";
				LoopControler = true;
			} else {
				Maneger.SendMessage ("ParticleLoopStatus",LoopControler);
				Effect_State = "Effect Loop:OFF";
				LoopControler = false;
			}
		}
	}
	
	
	
}