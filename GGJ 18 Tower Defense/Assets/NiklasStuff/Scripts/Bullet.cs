using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Transform homeTarget;
    [SerializeField]
    private float speed;
    public float damage = 10;

    public void Setup(Transform newTarget)
    {
        homeTarget = newTarget;
    }

    void Update()
    {

        if(homeTarget == null)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (homeTarget != null)
        {
            transform.LookAt(homeTarget);
            transform.position += transform.forward * speed;
        }
    }

}
