using UnityEngine;
using UnityEngine.Events;

public class ReturnButton : MonoBehaviour {

	[SerializeField] UnityEvent m_returnEvent;

	void Update(){
		if(Input.GetButtonDown("Cancel")){
			m_returnEvent.Invoke();
		}
	}
	
}
