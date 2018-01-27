using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Transform homeTarget;
    [SerializeField]
    private float speed;
    public float damage = 10;

    private float destroyAfterTime = 20;

    public void Setup(Transform newTarget)
    {
        homeTarget = newTarget;
    }

    void Update()
    {
        destroyAfterTime -= Time.deltaTime;
        if(homeTarget == null || destroyAfterTime < 0)
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
