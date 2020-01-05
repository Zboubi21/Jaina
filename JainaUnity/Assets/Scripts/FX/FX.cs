using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour {

	protected virtual void Start()
	{
		ParticleSystem ps = GetComponent<ParticleSystem>();		// On récupère le composant ParticleSystem et on le "met" dans une variable (ps) de type ParticleSystem 
		AudioSource audio = GetComponent<AudioSource>();		// De même pour l'AudioSource

		if( (audio != null) && (ps != null) )
		{					// Si le FX possède du son et un effet de particule alors : 
			if(audio.clip.length >= ps.main.duration)
			{			// Si la duré du son est plus longue ou égale à la duré de l'effet de particule alors :
				Destroy(this.gameObject, audio.clip.length);	// On détruit le FX lorsque l'audio est fini

			}
			else if(ps.main.duration >= audio.clip.length)
			{	// Sinon si la duré de l'effet de particule est plus longue ou égale à la duré du son alors :
				if(!ps.main.loop)
				{
					Destroy(this.gameObject, ps.main.duration);		// On détruit le FX lorsque l'effet de particule se termine
				}
			}
		}
		else
		{													// S'il manque ou en effet de particule ou du son alors :
			if(audio != null)
			{									// Si le FX possède un "AudioSource" alors :
				Destroy(this.gameObject, audio.clip.length);	// On détruit le FX à la fin du son 
			}
			if(ps != null)
			{	
				if(!ps.main.loop)
				{									// Si le FX possède un "ParticleSystem" alors :
					Destroy(this.gameObject, ps.main.duration);		// On détruit le gameObject à la fin de l'effet de particule
				}
			}
		}
	}
}
