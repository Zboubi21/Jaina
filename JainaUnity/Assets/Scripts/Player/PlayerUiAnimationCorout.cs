using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUiAnimationCorout : MonoBehaviour {
    
    PlayerManager m_playerManager;

    void Awake(){
        m_playerManager = GetComponent<PlayerManager>();
    }
    
	public IEnumerator ChangeSpellAlphaCorout(Image spellImg, Image cdImg, TextMeshProUGUI text, float fromAlpha, float toAlpha){
		
		float distance = Mathf.Abs(fromAlpha - toAlpha);
		float moveFracJourney = new float();
		float vitesse = distance / m_playerManager.m_powers.m_uI.m_uIAnimations.m_timeToFinish;
		Color desiredColor = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, toAlpha);

		while(spellImg.color != desiredColor){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			float alphaValue = Mathf.Lerp(fromAlpha, toAlpha, m_playerManager.m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			spellImg.color = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, alphaValue);
			cdImg.color = new Color(cdImg.color.r, cdImg.color.g, cdImg.color.b, alphaValue);
			text.color = new Color(text.color.r, text.color.g, text.color.b, alphaValue);
			yield return null;
		}
	}
	public IEnumerator MoveToYourNextPosition(RectTransform transformObject, Vector3 fromPosition, Vector3 toPosition, RectTransform firstSpellToTp = null, Image firstSpellImg = null, Image firstCdImg = null, TextMeshProUGUI firstText = null, float firstNewAlpha = 0, Image secondSpellImg = null, Image secondCdImg = null, TextMeshProUGUI secondText = null, float secondNewAlpha = 0){
		
		float distance = Vector3.Distance(fromPosition, toPosition);
		float moveFracJourney = new float();
		float vitesse = distance / m_playerManager.m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(transformObject.localPosition != toPosition) {
		// while(moveFracJourney < 1) {
			// Debug.Log("MoveToYourNextPosition | moveFracJourney = " + moveFracJourney);
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transformObject.localPosition = Vector3.Lerp(fromPosition, toPosition, m_playerManager.m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));

			yield return null;
		}
		if(firstSpellToTp != null){
			firstSpellToTp.localPosition = transformObject.localPosition;				// On TP le spell principal
			ChangeSpellAlpha(firstSpellImg, firstCdImg, firstText, firstNewAlpha);		// On enlève sa transparence
			ChangeSpellAlpha(secondSpellImg, secondCdImg, secondText, secondNewAlpha);	// On met transparent le spell secondaire
		}
	}
	public IEnumerator ChangeSpriteSize(RectTransform transformObject, Vector2 fromScale, Vector2 toScale){
		
		float distance = Vector3.Distance(fromScale, toScale);
		float moveFracJourney = new float();
		float vitesse = distance / m_playerManager.m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(transformObject.sizeDelta != toScale){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			transformObject.sizeDelta = Vector3.Lerp(fromScale, toScale, m_playerManager.m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			yield return null;
		}
	}
	public IEnumerator ChangeFontSize(TextMeshProUGUI textObject, float fromSize, float toSize){
		
		float distance = Mathf.Abs(fromSize - toSize);
		float moveFracJourney = new float();
		float vitesse = distance / m_playerManager.m_powers.m_uI.m_uIAnimations.m_timeToFinish;

		while(textObject.fontSize != toSize){
			moveFracJourney += (Time.deltaTime) * vitesse / distance;
			textObject.fontSize = Mathf.Lerp(fromSize, toSize, m_playerManager.m_powers.m_uI.m_uIAnimations.m_curveAnim.Evaluate(moveFracJourney));
			yield return null;
		}
	}

    public void ChangeSpellAlpha(Image spellImg, Image cdImg, TextMeshProUGUI text, float newAlpha){
		spellImg.color = new Color(spellImg.color.r, spellImg.color.g, spellImg.color.b, newAlpha);
		cdImg.color = new Color(cdImg.color.r, cdImg.color.g, cdImg.color.b, newAlpha);
		text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
	}
    public void On_StopAllCoroutines(){
        StopAllCoroutines();
    }

}
