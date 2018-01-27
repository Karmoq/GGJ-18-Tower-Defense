using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChangeSoundController : MonoBehaviour {

    private bool isPlaying;

    private AudioController ac;

    float xDir = 0;
    float yDir = 0;

    void Start()
    {
        ac = GetComponent<AudioController>();
    }

    void Update()
    {
        if (AirShipPlayerController.WindDirection.x > 0 && xDir < 0)
        {
            ac.Play();
        }
        if (AirShipPlayerController.WindDirection.x < 0 && xDir > 0)
        {
            ac.Play();
        }
        if (AirShipPlayerController.WindDirection.y > 0 && yDir < 0)
        {
            ac.Play();
        }
        if (AirShipPlayerController.WindDirection.y < 0 && yDir > 0)
        {
            ac.Play();
        }

        xDir = AirShipPlayerController.WindDirection.x;
        yDir = AirShipPlayerController.WindDirection.y;
    }

}
