using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollInitForce : MonoBehaviour
{

    [SerializeField]
    private float strength;
    [SerializeField]
    private float torqueAmp;

    Rigidbody rb;
    bool active = false;

    [SerializeField]
    private AudioController ac;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Activate();
        }
        if(active)
        {
            transform.localScale += Vector3.one * Time.deltaTime;
        }
    }

    private void ApplyForce()
    {
        /*Vector3 ranSphere = Random.onUnitSphere;
        ranSphere.y = upForce;
        ranSphere.x *= strength;
        ranSphere.z *= strength;
        rb.AddForce(ranSphere);*/
        Vector3 ranSphere = Random.insideUnitSphere;
        rb.AddTorque(ranSphere * torqueAmp);

        rb.AddForce(/*(Camera.main.transform.position - transform.position)*/ new Vector3(-1,5,-1) * strength);

    }

    public void Activate()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        ApplyForce();
        active = true;
        GetComponent<Animator>().SetTrigger("PlayAnimation");
        Destroy(gameObject, 5);
        ac.Play();
    }
}