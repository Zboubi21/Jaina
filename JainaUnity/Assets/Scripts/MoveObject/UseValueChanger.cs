using UnityEngine;

public class UseValueChanger : MonoBehaviour, IUseValueChanger
{
    protected ValueChanger m_valueChangerBase;
    // FloatData m_changeXPosition = new FloatData();

    protected bool m_detachAllObserverAutomatically = true;

    public virtual void Awake(){
        m_valueChangerBase = new ValueChanger(this);
    }

#region Observer setup
    public void AttachAllObservers()
    {
        AttachStartObserver();
        AttachWhileObserver();
        AttachEndObserver();
    }
    public void AttachStartObserver()
    {
        m_valueChangerBase.OnStartChangeValue += On_ValueChangerIsStart;
    }
    public void AttachWhileObserver()
    {
        m_valueChangerBase.OnWhileChangeValue += On_ValueChangerIsWhile;
    }
    public void AttachEndObserver()
    {
        m_valueChangerBase.OnEndChangeValue += On_ValueChangerIsEnd;
    }

    public void DetachAllObservers()
    {
        DetachStartObserver();
        DetachWhileObserver();
        DetachEndObserver();
    }
    public void DetachStartObserver()
    {
        m_valueChangerBase.OnStartChangeValue -= On_ValueChangerIsStart;
    }
    public void DetachWhileObserver()
    {
        m_valueChangerBase.OnWhileChangeValue -= On_ValueChangerIsWhile;
    }
    public void DetachEndObserver()
    {
        m_valueChangerBase.OnEndChangeValue -= On_ValueChangerIsEnd;
    }
#endregion //Observer setup

#region Observer Call
    public virtual void On_ValueChangerIsStart()
    {
        Debug.Log("StartToMove");
        if(m_detachAllObserverAutomatically){
            DetachStartObserver();
        }
    }
    public virtual void On_ValueChangerIsWhile()
    {
        Debug.Log("WhileMoving");
    }
    public virtual void On_ValueChangerIsEnd()
    {
        Debug.Log("EndMove");
        if(m_detachAllObserverAutomatically){
            DetachWhileObserver();
            DetachEndObserver();
        }
    }
#endregion //Observer Call

}
