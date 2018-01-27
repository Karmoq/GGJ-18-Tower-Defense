using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSoundController : MonoBehaviour {

    private bool isPlaying;

    private AudioController ac;

	void Start ()
    {
        ac = GetComponent<AudioController>();
	}
	
	void Update ()
    {
		if(AirShipPlayerController.WindDirection.magnitude > 0.2f)
        {
            if(!isPlaying)
            {
                ac.Play();
                isPlaying = true;
            }
        }
        else
        {
            if(isPlaying)
            {
                ac.Stop();
                isPlaying = false;
            }
        }
	}

}
