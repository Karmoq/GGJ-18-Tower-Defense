using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {

    public static ScreenShake S;

    [SerializeField]
    private float maxShake = 0.1f;
    [SerializeField]
    private float shakeLoss = 0.1f;
    [SerializeField]
    private float cameraSpeed = 0.1f;

    private float currentShake = 0;

    private bool init = false;

    Vector3 startPos;

    void Awake()
    {
        S = this;
        StartCoroutine(WaitStart());
    
}

    void FixedUpdate()
    {
        if (!init)
            return;
        Vector3 targetPos = Random.insideUnitSphere;

        transform.position = Vector3.Lerp(transform.position, startPos + targetPos * currentShake, cameraSpeed);
        currentShake = Mathf.Lerp(currentShake, 0, shakeLoss);

    }

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(0.2f);
        startPos = transform.position;
        init = true;
    }

    public void Shake(float intensity)
    {
        currentShake = intensity;
    }
}
