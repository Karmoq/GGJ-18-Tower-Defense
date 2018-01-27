using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticleController : MonoBehaviour {

    ParticleSystem.VelocityOverLifetimeModule ps;

    [SerializeField]
    private float lerpSpeed;

	void Start () {
        ps = GetComponent<ParticleSystem>().velocityOverLifetime;
	}
	
	void FixedUpdate ()
    {
        ps.x = Mathf.Lerp(ps.x.constant,AirShipPlayerController.WindDirection.x,lerpSpeed);
        ps.y = Mathf.Lerp(ps.y.constant, AirShipPlayerController.WindDirection.y, lerpSpeed);
        ps.z = Mathf.Lerp(ps.z.constant, AirShipPlayerController.WindDirection.z, lerpSpeed);
    }
}
