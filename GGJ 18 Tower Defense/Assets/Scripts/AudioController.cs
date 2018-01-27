using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {


	public AudioClip[] audioClp;
	AudioSource audioSrc;
	public bool RandomClip;
	public bool playOnStart;
	public bool RandomPitch;
	public float RandomPitchRange = 0.05f;
	public bool waitToFinish;
	
	void Start(){
		audioSrc = GetComponent<AudioSource>();
		audioSrc.clip = audioClp[0];
		if (playOnStart) {
			Play ();
		}
	}

	public void Play(){
		if (audioSrc.isPlaying) {
			if (waitToFinish) {
				return;
			} else {
			
			}
		}
		if (audioSrc.pitch != 1) {
			audioSrc.pitch = 1f;
		}
		if (RandomPitch) {
			pitchRandomizer (RandomPitchRange);
		}
		if (RandomClip && audioClp.Length > 1) {
			if (waitToFinish) {
				StartCoroutine (waitForSound ());
			} else {
				audioSrc.Stop ();
				clipRandomizer ();
			}
		}
		audioSrc.Play();

	}

	public void Stop(){
		audioSrc.Stop();
	}

	public void changeClip(int a){
		if (a < audioClp.Length) {
			audioSrc.clip = audioClp [a];
		} else {
			Debug.Log ("Tried to load audioclip, that does not exist. Check your changeClip() calls.");
		}
	}

	void clipRandomizer(){
		audioSrc.clip = audioClp[Random.Range(0,audioClp.Length)];
	}

	void pitchRandomizer(float f){
		audioSrc.pitch = audioSrc.pitch+Random.Range (-f, f);
	}

	IEnumerator waitForSound(){
		while (audioSrc.isPlaying) {
			yield return null;
		}
		clipRandomizer ();
	}
}
