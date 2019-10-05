using System.Collections;
using UnityEngine;

public class ValueChanger : MonoBehaviour
{

#region Constructor
    MonoBehaviour m_coroutineMonoBehaviour;
    public ValueChanger(MonoBehaviour coroutineMonoBehaviour)
    {
        m_coroutineMonoBehaviour = coroutineMonoBehaviour;
    }
#endregion //Constructor

#region Observer variables
    public delegate void StartChangeValue();
    public event StartChangeValue OnStartChangeValue;

    public delegate void WhileChangeValue();
    public event WhileChangeValue OnWhileChangeValue;

    public delegate void EndChangeValue();
    public event EndChangeValue OnEndChangeValue;
#endregion //Observer variables

#region Move Functions

    #region MovePosition
        public MoveObjectData MovePositionWithSpeed(Transform transformPosition, Vector3 toPosition, float changePositionSpeed)
        {
            MoveObjectData moveObjectData =  new MoveObjectData();
            m_coroutineMonoBehaviour.StartCoroutine(MovePositionCorout(moveObjectData, transformPosition, toPosition, changePositionSpeed));
            return moveObjectData;
        }
        public MoveObjectData MovePositionWithSpeed(Transform transformPosition, Vector3 toPosition, float changePositionSpeed, AnimationCurve animationCurve)
        {
            MoveObjectData moveObjectData =  new MoveObjectData();
            m_coroutineMonoBehaviour.StartCoroutine(MovePositionCorout(moveObjectData, transformPosition, toPosition, changePositionSpeed, animationCurve));
            return moveObjectData;
        }
        public MoveObjectData MovePositionWithTime(Transform transformPosition, Vector3 toPosition, float timeToReachNewValue)
        {
            MoveObjectData moveObjectData =  new MoveObjectData();
            float distance = Vector3.Distance(transformPosition.position, toPosition);
            float speed = distance / timeToReachNewValue;
            m_coroutineMonoBehaviour.StartCoroutine(MovePositionCorout(moveObjectData, transformPosition, toPosition, speed));
            return moveObjectData;
        }
        public MoveObjectData MovePositionWithTime(Transform transformPosition, Vector3 toPosition, float timeToReachNewValue, AnimationCurve animationCurve)
        {
            MoveObjectData moveObjectData =  new MoveObjectData();
            float distance = Vector3.Distance(transformPosition.position, toPosition);
            float speed = distance / timeToReachNewValue;
            m_coroutineMonoBehaviour.StartCoroutine(MovePositionCorout(moveObjectData, transformPosition, toPosition, speed, animationCurve));
            return moveObjectData;
        }
        IEnumerator MovePositionCorout(MoveObjectData moveObjectData, Transform moveTransform, Vector3 toPosition, float changeSpeed, AnimationCurve animationCurve = null)
        {

            Vector3 fromPosition = moveTransform.position;

            float moveFracJourney = 0;
            float distance = Vector3.Distance(fromPosition, toPosition);

            moveObjectData.IsRunning = true;
            moveObjectData.FracJourney = 0;

            On_StartChangeValue();

            while (moveTransform.position != toPosition)
            {
                moveFracJourney += (Time.deltaTime) * changeSpeed / distance;
                moveObjectData.FracJourney = moveFracJourney;

                if (animationCurve != null)
                {
                    moveTransform.position = Vector3.Lerp(fromPosition, toPosition, animationCurve.Evaluate(moveFracJourney));
                }
                else
                {
                    moveTransform.position = Vector3.Lerp(fromPosition, toPosition, moveFracJourney);
                }
                On_WhileChangeValue();
                yield return null;
            }
            moveObjectData.IsRunning = false;
            On_EndChangeValue();
        }
    #endregion //MovePosition

    #region ChangeFloat
        public FloatData ChangeFloatWithSpeed(float fromValue, float toValue, float speed)
        {
            FloatData changeFloatData =  new FloatData();
            m_coroutineMonoBehaviour.StartCoroutine(ChangeFloatCorout(changeFloatData, fromValue, toValue, speed));
            return changeFloatData;
        }  
        public FloatData ChangeFloatWithSpeed(float fromValue, float toValue, float speed, AnimationCurve changeValueCurve)
        {
            FloatData changeFloatData =  new FloatData();
            m_coroutineMonoBehaviour.StartCoroutine(ChangeFloatCorout(changeFloatData, fromValue, toValue, speed, changeValueCurve));
            return changeFloatData;
        } 
        public FloatData ChangeFloatWithTime(float fromValue, float toValue, float timeToReachNewValue)
        {
            FloatData changeFloatData =  new FloatData();
            float distance = Mathf.Abs(fromValue - toValue);
            float speed = distance / timeToReachNewValue;
            m_coroutineMonoBehaviour.StartCoroutine(ChangeFloatCorout(changeFloatData, fromValue, toValue, speed));
            return changeFloatData;
        }  
        public FloatData ChangeFloatWithTime(float fromValue, float toValue, float timeToReachNewValue, AnimationCurve changeValueCurve)
        {
            FloatData changeFloatData =  new FloatData();
            float distance = Mathf.Abs(fromValue - toValue);
            float speed = distance / timeToReachNewValue;
            m_coroutineMonoBehaviour.StartCoroutine(ChangeFloatCorout(changeFloatData, fromValue, toValue, speed, changeValueCurve));
            return changeFloatData;
        }           
        IEnumerator ChangeFloatCorout(FloatData changeFloatData, float fromValue, float toValue, float changeSpeedValue, AnimationCurve changeValueCurve = null)
        {
            changeFloatData.FloatValue = fromValue;
            float moveFracJourney = 0;

            float distance = Mathf.Abs(fromValue - toValue);

            changeFloatData.IsRunning = true;
            changeFloatData.FracJourney = 0;

            On_StartChangeValue();            

            while (changeFloatData.FloatValue != toValue)
            {
                moveFracJourney += (Time.deltaTime) * changeSpeedValue / distance;
                changeFloatData.FracJourney = moveFracJourney;

                if(changeValueCurve != null)
                {
                    changeFloatData.FloatValue = Mathf.Lerp(fromValue, toValue, changeValueCurve.Evaluate(moveFracJourney));
                }
                else
                {
                    changeFloatData.FloatValue = Mathf.Lerp(fromValue, toValue, moveFracJourney);
                }
                On_WhileChangeValue();                
                yield return null;
            }
            changeFloatData.IsRunning = false;
            On_EndChangeValue();            
        }
    #endregion //ChangeFloat

    #region MoveRotation
        public MoveObjectData MoveRotationWithTime(Transform transformRotation, Vector3 toRotation, float timeToReachNewValue, AnimationCurve animationCurve)
        {
            MoveObjectData moveObjectData =  new MoveObjectData();
            float distance = Vector3.Distance(transformRotation.localRotation.eulerAngles, toRotation);
            float speed = distance / timeToReachNewValue;
            m_coroutineMonoBehaviour.StartCoroutine(MoveRotationCorout(moveObjectData, transformRotation, toRotation, speed, animationCurve));
            return moveObjectData;
        }
        IEnumerator MoveRotationCorout(MoveObjectData moveObjectData, Transform rotateTransform, Vector3 toRotation, float changeSpeed, AnimationCurve animationCurve = null)
        {

            Vector3 fromRotation = rotateTransform.localRotation.eulerAngles;

            float moveFracJourney = 0;
            float distance = Vector3.Distance(fromRotation, toRotation);

            moveObjectData.IsRunning = true;
            moveObjectData.FracJourney = 0;

            On_StartChangeValue();

            while (rotateTransform.localRotation.eulerAngles != toRotation)
            {
                moveFracJourney += (Time.deltaTime) * changeSpeed / distance;
                moveObjectData.FracJourney = moveFracJourney;

                if (animationCurve != null)
                {
                    rotateTransform.localRotation = Quaternion.Euler(Vector3.Lerp(fromRotation, toRotation, animationCurve.Evaluate(moveFracJourney)));
                }
                else
                {
                    // moveTransform.rotation = Quaternion.Lerp(fromRotation, toRotation, moveFracJourney);
                    rotateTransform.localRotation = Quaternion.Euler(Vector3.Lerp(fromRotation, toRotation, moveFracJourney));
                }
                On_WhileChangeValue();
                yield return null;
            }
            moveObjectData.IsRunning = false;
            On_EndChangeValue();
        }
    #endregion //MoveRotation

#endregion //Move Functions 

#region Observer functions
    void On_StartChangeValue(){
        if(OnStartChangeValue != null){
            OnStartChangeValue();
        }
    }
    void On_WhileChangeValue(){
        if(OnWhileChangeValue != null){
            OnWhileChangeValue();
        }
    }
    void On_EndChangeValue(){
        if(OnEndChangeValue != null){
            OnEndChangeValue();
        }
    }
#endregion //Observer functions

}
