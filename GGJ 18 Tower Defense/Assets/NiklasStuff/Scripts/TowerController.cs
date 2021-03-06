﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TowerController : MonoBehaviour {

    private Transform currentTarget;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float range = 10;
    [SerializeField]
    private Transform firePos;
    [SerializeField]
    private float fireRate;

    private float currentReload = 0;

    private float towerLifeTime;

    private Vector3 anchorPosition;

    [SerializeField]
    private float lerpSpeed = 0.1f;

    [SerializeField]
    private Transform towerTransform;
    [SerializeField]
    private GameObject cannonParticle;

    private Animator anim;

    private AudioController acFire;

    [SerializeField]
    private GameObject flyAway;

    void Start()
    {
        anchorPosition = transform.position;
        anim = GetComponent<Animator>();
        acFire = GetComponent<AudioController>();
    }

	void Update ()
    {
        towerLifeTime -= Time.deltaTime;

        if(towerLifeTime <= 0)
        {
            Die();
        }

        float distance = 99999;
        List<Transform> targetList = TrainManager.S.Targets;
        currentTarget = null;
        for (int i = 0; i < targetList.Count; i++)
        {
            //Find closest target in range
            Vector3 targetPosition = targetList[i].position;
            targetPosition.y = 0;
            Vector3 towerPosition = towerTransform.position;
            towerPosition.y = 0;

            float targetDistance = Vector3.Distance(targetPosition, towerPosition);
            if (targetDistance < range)
            {
                if (distance > targetDistance)
                {
                    distance = targetDistance;
                    currentTarget = targetList[i];
                }
            }
        }

        //Fire logic
        currentReload -= Time.deltaTime;
        if(currentReload <= 0)
        {
            if(currentTarget != null)
            {
                //Fire at target
                GameObject bulletObject = Instantiate(bulletPrefab,firePos.position,firePos.rotation);
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.Setup(currentTarget);
                GameObject particleObject = Instantiate(cannonParticle, firePos.position, firePos.rotation);

                acFire.Play();

                currentReload = fireRate;
            }
        }

       
	}

    void FixedUpdate()
    {
        towerTransform.position = Vector3.Lerp(towerTransform.position,anchorPosition + AirShipPlayerController.WindDirection * AirShipPlayerController.WindStrength,lerpSpeed);
    }

    public void SetTowerLifeTime(float newLifeTime)
    {
        towerLifeTime = newLifeTime;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    void Die()
    {
        Instantiate(flyAway, transform.position, transform.rotation);
        anim.SetTrigger("Die");
        Destroy(gameObject, 1);
    }
}
