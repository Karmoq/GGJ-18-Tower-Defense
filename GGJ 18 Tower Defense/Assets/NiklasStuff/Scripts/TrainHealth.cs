using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainHealth : MonoBehaviour {

    [SerializeField]
    private float MaxHealth = 100;
    private float currentHealth;

    [SerializeField]
    private GameObject explosionObject;

    void Start()
    {
        currentHealth = MaxHealth;
        TrainManager.S.AddTarget(transform);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (explosionObject != null)
        {
            Instantiate(explosionObject, transform.position, explosionObject.transform.rotation);
        }
        ScreenShake.S.Shake(10);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        Bullet bullet = col.GetComponent<Bullet>();
        if(bullet != null)
        {
            TakeDamage(bullet.damage);
            Destroy(col.gameObject);
        }
    }

    void OnDestroy()
    {
        TrainManager.S.RemoveTarget(transform);
    }

    public float GetCurrentHealthPercentage()
    {
        return currentHealth/MaxHealth;
    }

}
