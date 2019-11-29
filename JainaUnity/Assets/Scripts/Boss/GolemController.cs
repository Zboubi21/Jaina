using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GolemStateEnum;

public class GolemController : MonoBehaviour
{
    [SerializeField] StateMachine m_sM = new StateMachine();

#region Event Functions
    void Awake()
    {
		SetupStateMachine();
    }

    void OnEnable()
    {

    }

    void Start()
    {
		ChangeState(GolemState.Idle);
    }

    void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    void Update()
    {
        m_sM.Update();
    }

#endregion

#region Private Functions
    void SetupStateMachine()
	{
		m_sM.AddStates(new List<IState> {
			new GolemIdleState(this),        // 0 = Idle
		});

		string[] golemStateNames = System.Enum.GetNames (typeof(GolemState));
		if(m_sM.States.Count != golemStateNames.Length){
    			Debug.LogError("You need to have the same number of State in GolemController and GolemStateEnum");
		}
	}

#endregion

#region Public Functions
    public void ChangeState(GolemState newState){
		m_sM.ChangeState((int)newState);
	}
#endregion

}
