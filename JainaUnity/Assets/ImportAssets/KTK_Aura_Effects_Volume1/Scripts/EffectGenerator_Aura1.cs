//======================================
/*
@autor ktk.kumamoto
@date 2015.3.23 create
@note EffectGenerator_Aura1
*/
//======================================


using UnityEngine;
using System.Collections;

public class EffectGenerator_Aura1 : MonoBehaviour {
	
	public float hSliderValue = 0.5F;
	public Material FloorMat;

	private GameObject Eff_Point;

	public GameObject[] Effect_List;

	private Vector2 scrollViewVector = Vector2.zero;
	
	private Rect scrollViewRect = new Rect(0, 20, 170, 500);
	
	private Rect scrollViewAllRect = new Rect (40, 70, 100, 1000);

	public bool LoopStatus = true; // true = loop, false = unloop

	void Awake() {
		Eff_Point = GameObject.Find("Eff_Point");
	}

	void Update() {
		if(FloorMat != null){
			FloorMat.color = new Color(hSliderValue, hSliderValue, hSliderValue, 1.0f);
		}
	}

	void OnGUI() {

		hSliderValue = GUI.HorizontalSlider(new Rect(170, 20, 100, 30), hSliderValue, 0.0F, 1.0F);
		GUI.Label(new Rect(170, 50,  200, 20), "FloorBrightness: " + hSliderValue);

		scrollViewVector = GUI.BeginScrollView(scrollViewRect , scrollViewVector, scrollViewAllRect);
		
		for(int i = 0; i < Effect_List.Length; i++)
		{
			if(Effect_List[i] != null){
				if (GUI.Button(new Rect(50, 70 + i * 40, 140, 30), Effect_List[i].name))
				{
		
					foreach(Transform child in Eff_Point.transform) {
						GameObject.Destroy(child.gameObject);
					}
					GameObject clone1 = Instantiate(Effect_List[i], Eff_Point.transform.position, Quaternion.identity) as GameObject;
					if(LoopStatus == false){
						clone1.SendMessage("loopEffectControl",false);
					}
					clone1.transform.rotation = Quaternion.Euler(0,  0 , 0 );
					clone1.transform.parent = Eff_Point.transform;


				}
			}
		}
				
		GUI.EndScrollView();

	}

	void ParticleLoopStatus(bool status){
		LoopStatus = status;
	}


}
