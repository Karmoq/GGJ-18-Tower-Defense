using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {

    [SerializeField]
    private Transform cannon;
    private TowerController tc;

    private Quaternion rotTarget = Quaternion.Euler(0, 0, 0);
    private float cooldown = 0;

    void Awake()
    {
        tc = GetComponent<TowerController>();
        Update();
    }

	void Update ()
    {
        Transform target = tc.GetTarget();
        if (target != null)
        {
            cannon.LookAt(target);
        }
        else
        {
            cooldown -= 1;
            if (cooldown < 0)
            {
                cooldown = Random.Range(150, 350);
                rotTarget = Quaternion.Euler(Random.Range(0.0f,15.0f), Random.Range(0f, 360f), 0);
            }
            cannon.rotation = Quaternion.Slerp(cannon.rotation, rotTarget, 0.02f);
        }
	}
}
